var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.production.json").AddEnvironmentVariables();

// Enable from any origin
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy => policy.AllowAnyOrigin());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseDeveloperExceptionPage();
//if (app.Environment.IsDevelopment())
//{
//  app.UseDeveloperExceptionPage();
//}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();