var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");
var sqldb = sql.AddDatabase("master");

builder.AddProject<Projects.NOS_Engineering_Challenge_API>("nos-engineering-challenge-api")
    .WithReference(sqldb);

builder.Build().Run();
