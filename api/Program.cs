using Abschlussprojekt.Services;
using Polly;
using Polly.Retry;
using Abschlussprojekt.Formatters;

var builder = WebApplication.CreateBuilder(args);


// 1. Controller hinzufügen
builder.Services.AddControllers(
    options =>
    {
        options.OutputFormatters.Add(new CsvOutputFormatter());
    }
)
    .AddXmlSerializerFormatters(); // XML-Formatter hinzufügen

// Response Caching-Middleware hinzufügen
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});

// 2. Swagger/OpenAPI hinzufügen
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// 3. Service registrieren (Aufgabe 5)
// Wir nutzen "AddSingleton", damit die Liste im Speicher bleibt, solange die App läuft!
builder.Services.AddSingleton<IStudentService, StudentService>();


//Queue & Worker registrieren
builder.Services.AddSingleton<IStudentEventQueue, InMemoryStudentEventQueue>();
builder.Services.AddHostedService<StudentEventWorker>();

// ExternalUnstableService mit Polly Retry Policy registrieren
builder.Services.AddSingleton<ExternalUnstableService>();

var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * attempt)
    );

builder.Services.AddSingleton<AsyncRetryPolicy>(retryPolicy);
var app = builder.Build();

app.UseHttpsRedirection();


// Pipeline Konfiguration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseResponseCaching();
// Custom Middleware für Cache-Control Header
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(10)
        };
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
        new string[] { "Accept-Encoding" };

    await next();
});
app.MapGet("/info", () => $"Response from API instance on port {app.Urls.FirstOrDefault()?.Split(':').Last()}");

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();