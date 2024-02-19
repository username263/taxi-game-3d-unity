using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using TaxiGame3D.Server.Database;
using TaxiGame3D.Server.Repositories;
using TaxiGame3D.Server.Services;
using TaxiGame3D.Server.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = builder
            .Configuration
            .GetSection("Jwt")
            .Get<JwtSettings>()!;
    });
builder.Services.AddTransient<TokenService>();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database")
);
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddSingleton<TemplateService>();

builder.Services.AddControllers().AddJsonOptions(
    // JSON으로 변환할 때 카멜식으로 자동변환 안 하게 됨
    options => options.JsonSerializerOptions.PropertyNamingPolicy = null
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id= "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
