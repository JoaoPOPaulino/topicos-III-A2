using A2.Data;
using A2.Service;
using A2.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// 1. CONFIGURAÇÃO BASE
// ------------------------------------

// 1.1. Cache em memória (necessário para ExchangeRateService)
builder.Services.AddMemoryCache();

// 1.2. Configuração do EF Core e DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1.3. Configuração CORS (para integração Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ------------------------------------
// 2. POLÍTICA DE RESILIÊNCIA (POLLY)
// ------------------------------------

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 5xx, 408, network failures
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests) // 429
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"⚠️ Tentativa {retryAttempt} falhou. Aguardando {timespan.TotalSeconds}s...");
            }
        );
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromMinutes(1),
            onBreak: (outcome, duration) =>
            {
                Console.WriteLine($"🔴 Circuit Breaker ABERTO por {duration.TotalSeconds}s");
            },
            onReset: () => Console.WriteLine("🟢 Circuit Breaker FECHADO")
        );
}

// ------------------------------------
// 3. REGISTRO DE CLIENTES HTTP
// ------------------------------------

// 3.1. Cliente HTTP para AwesomeAPI com Polly
builder.Services
    .AddHttpClient("AwesomeApiCambiaria", client =>
    {
        client.BaseAddress = new Uri("https://economia.awesomeapi.com.br/");
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// 3.2. HttpClientFactory genérico (para HolidayService e outros)
builder.Services.AddHttpClient();

// ------------------------------------
// 4. REGISTRO DE SERVIÇOS DE NEGÓCIO
// ------------------------------------

// ✅ Serviços principais (SEM DUPLICATAS)
builder.Services.AddScoped<ISolicitacaoAdiantamentoService, SolicitacaoAdiantamentoService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IPrestacaoContasService, PrestacaoContasService>();

// ✅ CRÍTICO: Registrar CurrencyService como Scoped (NÃO como HttpClient)
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

// ------------------------------------
// 5. CONFIGURAÇÃO MVC E SWAGGER
// ------------------------------------

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------------------
// 6. PIPELINE HTTP
// ------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAngularApp"); // ✅ CORS antes de Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();