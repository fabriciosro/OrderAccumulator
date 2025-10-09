using OrderAccumulator.Infrastructure;
using OrderAccumulator.Presentation;

var builder = WebApplication.CreateBuilder(args);


// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Add layers
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order Accumulator API", Version = "v1" });
});

// add cors para desenv
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// isso pra desenvolvimento
app.UseCors("AllowAll");

app.MapControllers();

var port = 5000; 
var url = $"https://localhost:{port}";

try
{
    app.Run();
    Console.WriteLine("OrderAccumulator started successfully on port 5000");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to start OrderAccumulator: {ex.Message}");
    throw;
}