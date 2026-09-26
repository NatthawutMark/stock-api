using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using stock_api.Models;
using System.Data;
using Npgsql;
using stock_api.Repositories.Dapper;
using stock_api.Repositories.EF;
using stock_api.Interfaces;
using stock_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<DapperUnitOfWork>();
builder.Services.AddScoped<EfUnitOfWork>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Register the Database Context
builder.Services.AddDbContext<DbContexts>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// ลงทะเบียน IDbConnection แบบ Scoped
#region ServicesDapper
builder.Services.AddScoped<IDbConnection>(sp => 
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // ระบุ URL ของหน้าบ้าน
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // ใส่เพิ่มหากมีการส่ง Cookie หรือ Credentials
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // เพิ่มบรรทัดนี้
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
