using Abschlussprojekt.Services;


var builder = WebApplication.CreateBuilder(args);

// 1. Controller hinzufügen
builder.Services.AddControllers();

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

var app = builder.Build();

// Pipeline Konfiguration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");



// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();