using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Demoulas_ProfitSharing_Api>("DemoulasProfitSharingApi");

builder.Build().Run();
