using A2.Data;
using A2.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviço de cache em memória (necessário para ExchangeRateService)
builder.Services.AddMemoryCache();

// ------------------------------------
// 1. CONFIGURAÇÃO BASE
// ------------------------------------

// Configuração do EF Core e DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------------
// 2. CONFIGURAÇÃO DE POLÍTICA DE RESILIÊNCIA (POLLY)
// ------------------------------------

// Função que define a política de retentativa
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        // Lida com erros 5xx e timeouts
        .HandleTransientHttpError()
        // Lida com o erro 429
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        // Usando a sobrecarga que funciona no seu ambiente (retorno de TimeSpan)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
            {
                // Não temos acesso ao resultado (429) aqui, mas a política já lida com o 429
                // Vamos usar a sobrecarga mais simples e a política cuidará do Rate Limit
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            }
        );
}

// ------------------------------------
// 3. REGISTRO DE SERVIÇOS E CLIENTES HTTP
// ------------------------------------

// Registro principal dos serviços de lógica de negócio
builder.Services.AddScoped<ISolicitacaoAdiantamentoService, SolicitacaoAdiantamentoService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

// Registra o cliente HTTP para a AwesomeAPI com a política de Polly (simples)
builder.Services
    .AddHttpClient("AwesomeApiCambiaria")
    // Usa o AddPolicyHandler com o método que retorna a política de retry
    .AddPolicyHandler(GetRetryPolicy())
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

// Registra um cliente HTTP padrão (útil para o HolidayService)
builder.Services.AddHttpClient();

// ------------------------------------
// 4. CONFIGURAÇÃO MVC E SWAGGER
// ------------------------------------

// Adiciona suporte a Controllers (necessário para as APIs e Views)
builder.Services.AddControllersWithViews();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// ------------------------------------
// 5. CONFIGURAÇÃO DO PIPELINE HTTP
// ------------------------------------

// Configuração do Swagger Middleware (Apenas em Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();