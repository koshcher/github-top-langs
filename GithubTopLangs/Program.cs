var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var portExists = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port);
if (portExists)
{
  builder.WebHost.ConfigureKestrel(options =>
  {
    options.ListenAnyIP(port);
  });
}

// Enable from any origin
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy => policy.AllowAnyOrigin());
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();