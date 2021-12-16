using EzRabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() {Title = "WebSample", Version = "v1"}); });

builder.Services.AddEzRabbitMQ()
    .AddScoped<IAdditionService, AdditionService>()
    .AddHostedService<RpcServerHandler>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebSample v1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();