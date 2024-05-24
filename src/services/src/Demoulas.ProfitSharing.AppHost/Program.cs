var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Demoulas_ProfitSharing_Api>("demoulas-profitsharing-api");

builder.Build().Run();
