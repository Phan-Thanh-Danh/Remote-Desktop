using ExamGuard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IExamGuardService, ExamGuardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteCors", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
              {
                  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                  {
                      return false;
                  }

                  return (uri.Host == "localhost" || uri.Host == "127.0.0.1")
                      && uri.Port >= 5173
                      && uri.Port <= 5199;
              })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ViteCors");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
