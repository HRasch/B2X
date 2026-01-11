var builder = DistributedApplication.CreateBuilder(args);

// Add services (only Aspire projects)
builder.AddProject<Projects.B2X_Identity_API>("identity-api");
builder.AddProject<Projects.B2X_Tenancy_API>("tenancy-api");
builder.AddProject<Projects.B2X_Localization_API>("localization-api");
builder.AddProject<Projects.B2X_Catalog_API>("catalog-api");
builder.AddProject<Projects.B2X_Theming_API>("theming-api");
builder.AddProject<Projects.B2X_Store>("store");
builder.AddProject<Projects.B2X_Admin>("admin");

// Add infrastructure
builder.AddRedis("redis");
builder.AddPostgres("postgres");
builder.AddRabbitMQ("rabbitmq");
builder.AddElasticsearch("elasticsearch");

builder.Build().Run();