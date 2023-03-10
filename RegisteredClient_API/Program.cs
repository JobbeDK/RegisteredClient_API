using RegisteredClient_API;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to the container.
builder.Configuration.AddJsonFile("Properties/appSecrets.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

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

app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
