using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;
using UcakRezervasyon.DataAccess.Data;
using UcakRezervasyon.DataAccess.Repositories;
using UcakRezervasyon.Business.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Uçak Rezervasyon API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { jwtScheme, new string[] {} }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection();

var jwt = configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
});

builder.Services.AddScoped(typeof(UcakRezervasyon.DataAccess.Repositories.IGenericRepository<>), typeof(UcakRezervasyon.DataAccess.Repositories.GenericRepository<>));
builder.Services.AddScoped<UcakRezervasyon.DataAccess.Repositories.IUnitOfWork, UcakRezervasyon.DataAccess.Repositories.UnitOfWork>();
builder.Services.AddScoped<UcakRezervasyon.Business.Services.IUserService, UcakRezervasyon.Business.Services.UserService>();
builder.Services.AddScoped<UcakRezervasyon.Business.Services.IAuthService, UcakRezervasyon.Business.Services.AuthService>();

builder.Services.AddMvc();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseMiddleware<UcakRezervasyon.Api.Middlewares.ExceptionHandlingMiddleware>();
app.UseMiddleware<UcakRezervasyon.Api.Middlewares.MaintenanceMiddleware>();
app.UseMiddleware<UcakRezervasyon.Api.Middlewares.RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
