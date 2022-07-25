var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Enable from any origin
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy => policy.AllowAnyOrigin());
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsProduction())
{
  var port = Environment.GetEnvironmentVariable("PORT");
  app.Urls.Add($"http://*:{port}");
}

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();