IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Demoulas_ProfitSharing_Api>("demoulas-profitsharing-api");

await builder.Build().RunAsync();
