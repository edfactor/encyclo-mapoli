IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Demoulas_ProfitSharing_Api>("demoulas-profitsharing-api");

builder.AddNpmApp("demoulas-profitsharing-ui", "../../../ui/", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpsEndpoint(env: "PORT", port: 3100)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
