using TaxiGame3D.Server.Database;
using TaxiGame3D.Server.Repositories;
using TaxiGame3D.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
