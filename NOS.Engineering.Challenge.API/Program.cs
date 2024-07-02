using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Context;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

builder.AddSqlServerDbContext<EFSQLServerContext>("sql");

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();
    
app.Run();