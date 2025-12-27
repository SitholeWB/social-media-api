var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.SocialMedia_API>("api");

builder.AddNpmApp("admin", "../SocialMedia.Admin", "dev")
    .WithReference(api)
    .WithHttpEndpoint(targetPort: 5173, port: 5174, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddProject<Projects.SocialMedia_Files_API>("socialmedia-files-api");

await builder.Build().RunAsync();