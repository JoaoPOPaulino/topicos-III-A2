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
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviço de cache em memória (necessário para ExchangeRateService)
builder.Services.AddMemoryCache();

// ------------------------------------
// 1. CONFIGURAÇÃO BASE
// ------------------------------------

// Configuração do EF Core e DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1.2. Configuração CORS (CRÍTICO para integração Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            // Permite requisições do front-end Angular local
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

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
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
            {
                // Retentativa exponencial padrão: 2s, 4s, 8s
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            }
        );
}

// ------------------------------------
// 3. REGISTRO DE SERVIÇOS E CLIENTES HTTP
// ------------------------------------

// Registro principal dos serviços de lógica de negócio (REMOVENDO DUPLICATAS)
builder.Services.AddScoped<ISolicitacaoAdiantamentoService, SolicitacaoAdiantamentoService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IPrestacaoContasService, PrestacaoContasService>();

// Registra o cliente HTTP para a AwesomeAPI com a política de Polly (simples)
builder.Services
    .AddHttpClient("AwesomeApiCambiaria")
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

// 5.1. Usar o Middleware CORS
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();