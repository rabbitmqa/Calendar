//https://calendartaskerapp-awe7dpa5engueccr.israelcentral-01.azurewebsites.net/api/calendar

using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Allows any origin to access the API
              .AllowAnyMethod() // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allows any headers
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Apply CORS policy to all endpoints
app.MapControllers();

app.Run();
