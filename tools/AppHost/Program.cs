var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pg")
                .AddDatabase("pgdb");

builder.AddProject<Projects.Api>("api")
        .WithReference(db);

builder.Build().Run();
