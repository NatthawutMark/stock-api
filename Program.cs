using Scalar.AspNetCore;

using System.Data;
using Npgsql;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using stock_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHostedService<TokenCleanupService>();

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

builder.Services.AddAuthentication(options =>
{
    // บังคับให้ระบบใช้ JWT เป็นค่าเริ่มต้นในการตรวจสอบสิทธิ์
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // ตั้ง false ไว้ก่อนสำหรับตอนรันเทสบน localhost (ถ้าขึ้น Production ค่อยแก้เป็น true)
    // 
    // นี่คือส่วน TokenValidationParameters ครับ
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // ตรวจสอบผู้ออก Token
        ValidateAudience = true, // ตรวจสอบผู้รับ Token
        ValidateLifetime = true, // ตรวจสอบวันหมดอายุ (สำคัญมากสำหรับการทำ Refresh Token)
        ValidateIssuerSigningKey = true, // ตรวจสอบลายเซ็น (Secret Key)

        // ดึงค่ามาจาก appsettings.json
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))

    };
});
// builder.Services.AddSwaggerGen(options =>
// {
//     // บังคับให้ Swagger ใช้ชื่อเต็มของ Class เช่น "stock_api.Request.MastWarehouseRequest+reqFields"
//     options.CustomSchemaIds(type => type.FullName);
// });

var app = builder.Build();
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // เพิ่มบรรทัดนี้
    // app.UseSwagger();
    // app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    // {
    //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //     options.RoutePrefix = string.Empty;
    // });
}
app.UseCors("AllowFrontend");

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
