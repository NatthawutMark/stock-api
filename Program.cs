using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using back_stock.Models;
using System.Data;
using Npgsql;
using back_stock.Repositories.Dapper; // เพิ่ม namespace ให้ถูกต้อง
using back_stock.Repositories.EF; // เพิ่ม namespace ให้ถูกต้อง

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<DapperUnitOfWork>();
builder.Services.AddScoped<EfUnitOfWork>();

// Register the Database Context
builder.Services.AddDbContext<DbContexts>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// ลงทะเบียน IDbConnection แบบ Scoped
builder.Services.AddScoped<IDbConnection>(sp => 
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
