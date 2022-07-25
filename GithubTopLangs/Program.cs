var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.UseKestrel(serverOptions =>
{
  serverOptions.ListenAnyIP(4000);
  serverOptions.ListenAnyIP(4001, listenOptions => listenOptions.UseHttps());
});

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