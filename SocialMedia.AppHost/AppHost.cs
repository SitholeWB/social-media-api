var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SocialMedia_API>("api");

builder.Build().Run();
