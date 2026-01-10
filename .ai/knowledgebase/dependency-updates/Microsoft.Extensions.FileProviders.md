---
docid: KB-112
title: Microsoft.Extensions.FileProviders
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.FileProviders

**Version:** 10.0.1 (Abstractions)  
**Package:** [Microsoft.Extensions.FileProviders.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Abstractions)  
**Documentation:** [File Providers in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/file-providers)

## Overview

Microsoft.Extensions.FileProviders provides abstractions and implementations for accessing file systems in a platform-agnostic way. File providers are used throughout ASP.NET Core to abstract file system access, enabling applications to work with files from various sources including the physical file system, embedded resources, and composite sources.

The library provides a consistent API for reading files, enumerating directories, and monitoring file changes, regardless of the underlying storage mechanism. This abstraction is essential for building flexible applications that can work with different file sources in development, testing, and production environments.

## Key Features

- **Platform-Agnostic File Access**: Unified API for accessing files from different sources
- **Change Monitoring**: Watch files and directories for changes with IChangeToken
- **Multiple Implementations**: Physical files, embedded resources, composite providers
- **Glob Pattern Support**: Wildcard patterns for file matching and monitoring
- **ASP.NET Core Integration**: Built-in integration with web framework features
- **Performance Optimized**: Efficient file enumeration and change detection

## Core Components

### IFileProvider Interface
- **GetFileInfo(path)**: Returns IFileInfo for a specific file
- **GetDirectoryContents(path)**: Returns IDirectoryContents for a directory
- **Watch(filter)**: Returns IChangeToken for monitoring file changes

### IFileInfo Interface
- **Exists**: Boolean indicating if the file exists
- **IsDirectory**: Boolean indicating if this is a directory
- **Name**: File or directory name
- **Length**: File size in bytes (0 for directories)
- **LastModified**: Last modification timestamp
- **CreateReadStream()**: Creates a readable stream for the file content

### IDirectoryContents Interface
- **Exists**: Boolean indicating if the directory exists
- **GetEnumerator()**: Enumerates IFileInfo objects in the directory

## B2X Integration

The B2X platform extensively uses Microsoft.Extensions.FileProviders for various file-related operations across the application stack. This enables consistent file access patterns and change monitoring capabilities.

### Static File Serving in Store Gateway

```csharp
// src/backend/Store/Gateway/Program.cs
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configure static file serving with custom file provider
var staticFilesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(staticFilesPath))
{
    Directory.CreateDirectory(staticFilesPath);
}

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(staticFilesPath));

// Configure static files middleware
builder.Services.Configure<StaticFileOptions>(options =>
{
    options.FileProvider = new PhysicalFileProvider(staticFilesPath);
    options.RequestPath = "/static";

    // Cache static files for 1 year
    options.OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "public,max-age=31536000";
    };
});

var app = builder.Build();

// Serve static files
app.UseStaticFiles();

// Serve static files from custom path
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFilesPath),
    RequestPath = "/assets"
});

app.Run();
```

### Embedded Resources in Admin Gateway

```csharp
// src/backend/Admin/Gateway/Program.cs
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add embedded file provider for admin templates
var embeddedProvider = new ManifestEmbeddedFileProvider(
    typeof(Program).Assembly,
    "AdminGateway.Templates");

builder.Services.AddSingleton<IFileProvider>("EmbeddedTemplates", embeddedProvider);

// Register composite provider combining physical and embedded files
var physicalProvider = new PhysicalFileProvider(
    Path.Combine(builder.Environment.ContentRootPath, "Templates"));

var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
builder.Services.AddSingleton<IFileProvider>(compositeProvider);

var app = builder.Build();
```

### CMS Template Management

```csharp
// src/backend/Management/CMS/Services/TemplateService.cs
using Microsoft.Extensions.FileProviders;

public class TemplateService
{
    private readonly IFileProvider _templateProvider;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(
        IFileProvider templateProvider,
        ILogger<TemplateService> logger)
    {
        _templateProvider = templateProvider;
        _logger = logger;
    }

    public async Task<string> GetTemplateContentAsync(string templateName, CancellationToken cancellationToken = default)
    {
        var fileInfo = _templateProvider.GetFileInfo($"Templates/{templateName}.html");

        if (!fileInfo.Exists)
        {
            throw new TemplateNotFoundException($"Template '{templateName}' not found");
        }

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        _logger.LogInformation("Loaded template {TemplateName}, size: {Size} bytes",
            templateName, fileInfo.Length);

        return content;
    }

    public IEnumerable<string> GetAvailableTemplates()
    {
        var templatesDir = _templateProvider.GetDirectoryContents("Templates");

        if (!templatesDir.Exists)
        {
            return Enumerable.Empty<string>();
        }

        return templatesDir
            .Where(f => !f.IsDirectory && f.Name.EndsWith(".html"))
            .Select(f => Path.GetFileNameWithoutExtension(f.Name));
    }

    public async Task WatchTemplateChangesAsync(Func<string, Task> onTemplateChanged)
    {
        var changeToken = _templateProvider.Watch("Templates/*.html");

        changeToken.RegisterChangeCallback(async state =>
        {
            var callback = (Func<string, Task>)state;
            await callback("Templates changed");
        }, onTemplateChanged);
    }
}

// Registration in CMS module
public static class CmsServiceCollectionExtensions
{
    public static IServiceCollection AddCmsTemplates(this IServiceCollection services, IConfiguration configuration)
    {
        // Physical file provider for custom templates
        var customTemplatesPath = configuration["Cms:CustomTemplatesPath"] ?? "CustomTemplates";
        var physicalProvider = new PhysicalFileProvider(
            Path.Combine(AppContext.BaseDirectory, customTemplatesPath));

        // Embedded file provider for default templates
        var embeddedProvider = new ManifestEmbeddedFileProvider(
            typeof(TemplateService).Assembly,
            "Cms.Templates");

        // Composite provider
        var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
        services.AddSingleton<IFileProvider>(compositeProvider);

        services.AddSingleton<TemplateService>();

        return services;
    }
}
```

### Media File Management

```csharp
// src/backend/Shared/Infrastructure/Media/MediaFileProvider.cs
using Microsoft.Extensions.FileProviders;

public class MediaFileProvider : IFileProvider
{
    private readonly IFileProvider _physicalProvider;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<MediaFileProvider> _logger;

    public MediaFileProvider(
        string rootPath,
        ITenantContext tenantContext,
        ILogger<MediaFileProvider> logger)
    {
        _physicalProvider = new PhysicalFileProvider(rootPath);
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var tenantPath = GetTenantPath(subpath);
        var fileInfo = _physicalProvider.GetFileInfo(tenantPath);

        _logger.LogDebug("Accessing media file: {Path}, exists: {Exists}",
            tenantPath, fileInfo.Exists);

        return fileInfo;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var tenantPath = GetTenantPath(subpath);
        return _physicalProvider.GetDirectoryContents(tenantPath);
    }

    public IChangeToken Watch(string filter)
    {
        var tenantFilter = GetTenantPath(filter);
        return _physicalProvider.Watch(tenantFilter);
    }

    private string GetTenantPath(string path)
    {
        var tenantId = _tenantContext.Tenant?.Id ?? "default";
        return Path.Combine(tenantId, path.TrimStart('/'));
    }
}

// Registration
public static class MediaServiceCollectionExtensions
{
    public static IServiceCollection AddMediaFileProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var mediaRootPath = configuration["Media:RootPath"] ?? Path.Combine(AppContext.BaseDirectory, "Media");

        services.AddSingleton<IFileProvider>(sp =>
        {
            var tenantContext = sp.GetRequiredService<ITenantContext>();
            var logger = sp.GetRequiredService<ILogger<MediaFileProvider>>();
            return new MediaFileProvider(mediaRootPath, tenantContext, logger);
        });

        return services;
    }
}
```

### File Change Monitoring for Configuration

```csharp
// src/backend/Shared/Infrastructure/Configuration/FileWatcherConfigurationProvider.cs
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

public class FileWatcherConfigurationProvider : ConfigurationProvider
{
    private readonly IFileProvider _fileProvider;
    private readonly string _configFilePath;
    private readonly ILogger<FileWatcherConfigurationProvider> _logger;
    private IDisposable _changeTokenRegistration;

    public FileWatcherConfigurationProvider(
        IFileProvider fileProvider,
        string configFilePath,
        ILogger<FileWatcherConfigurationProvider> logger)
    {
        _fileProvider = fileProvider;
        _configFilePath = configFilePath;
        _logger = logger;
    }

    public override void Load()
    {
        try
        {
            var fileInfo = _fileProvider.GetFileInfo(_configFilePath);
            if (fileInfo.Exists)
            {
                using var stream = fileInfo.CreateReadStream();
                Load(stream);
            }

            // Set up file watching
            _changeTokenRegistration?.Dispose();
            var changeToken = _fileProvider.Watch(_configFilePath);
            _changeTokenRegistration = changeToken.RegisterChangeCallback(
                _ => OnConfigFileChanged(),
                null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration from {Path}", _configFilePath);
        }
    }

    private void OnConfigFileChanged()
    {
        _logger.LogInformation("Configuration file {Path} changed, reloading", _configFilePath);
        Load();
        OnReload();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _changeTokenRegistration?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### Custom File Provider for Cloud Storage

```csharp
// src/backend/Shared/Infrastructure/FileProviders/CloudFileProvider.cs
using Microsoft.Extensions.FileProviders;

public class CloudFileProvider : IFileProvider
{
    private readonly ICloudStorageService _cloudStorage;
    private readonly ILogger<CloudFileProvider> _logger;

    public CloudFileProvider(
        ICloudStorageService cloudStorage,
        ILogger<CloudFileProvider> logger)
    {
        _cloudStorage = cloudStorage;
        _logger = logger;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        return new CloudFileInfo(_cloudStorage, subpath, _logger);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return new CloudDirectoryContents(_cloudStorage, subpath, _logger);
    }

    public IChangeToken Watch(string filter)
    {
        // Cloud storage typically doesn't support real-time change notifications
        // Return a token that never changes, or implement polling-based monitoring
        return NullChangeToken.Singleton;
    }
}

public class CloudFileInfo : IFileInfo
{
    private readonly ICloudStorageService _cloudStorage;
    private readonly string _path;
    private readonly ILogger _logger;

    public CloudFileInfo(
        ICloudStorageService cloudStorage,
        string path,
        ILogger logger)
    {
        _cloudStorage = cloudStorage;
        _path = path;
        _logger = logger;
    }

    public bool Exists => _cloudStorage.FileExistsAsync(_path).GetAwaiter().GetResult();

    public bool IsDirectory => false;

    public string Name => Path.GetFileName(_path);

    public long Length => _cloudStorage.GetFileSizeAsync(_path).GetAwaiter().GetResult();

    public DateTimeOffset LastModified => _cloudStorage.GetLastModifiedAsync(_path).GetAwaiter().GetResult();

    public string PhysicalPath => null;

    public Stream CreateReadStream()
    {
        return _cloudStorage.DownloadFileAsync(_path).GetAwaiter().GetResult();
    }
}

public class CloudDirectoryContents : IDirectoryContents
{
    private readonly ICloudStorageService _cloudStorage;
    private readonly string _path;
    private readonly ILogger _logger;

    public CloudDirectoryContents(
        ICloudStorageService cloudStorage,
        string path,
        ILogger logger)
    {
        _cloudStorage = cloudStorage;
        _path = path;
        _logger = logger;
    }

    public bool Exists => true; // Assume directory exists if we're querying it

    public IEnumerator<IFileInfo> GetEnumerator()
    {
        var files = _cloudStorage.ListFilesAsync(_path).GetAwaiter().GetResult();
        return files.Select(file => new CloudFileInfo(_cloudStorage, file, _logger)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

### File Provider Extensions

```csharp
// src/backend/Shared/Infrastructure/FileProviders/FileProviderExtensions.cs
using Microsoft.Extensions.FileProviders;

public static class FileProviderExtensions
{
    public static async Task<string> ReadAllTextAsync(this IFileProvider fileProvider, string path)
    {
        var fileInfo = fileProvider.GetFileInfo(path);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<byte[]> ReadAllBytesAsync(this IFileProvider fileProvider, string path)
    {
        var fileInfo = fileProvider.GetFileInfo(path);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        using var stream = fileInfo.CreateReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public static IEnumerable<string> GetFiles(this IFileProvider fileProvider, string path, string searchPattern = "*")
    {
        var directoryContents = fileProvider.GetDirectoryContents(path);
        if (!directoryContents.Exists)
        {
            return Enumerable.Empty<string>();
        }

        return directoryContents
            .Where(f => !f.IsDirectory && MatchesPattern(f.Name, searchPattern))
            .Select(f => Path.Combine(path, f.Name));
    }

    public static IChangeToken WatchMultiple(this IFileProvider fileProvider, params string[] patterns)
    {
        var tokens = patterns.Select(p => fileProvider.Watch(p)).ToArray();
        return new CompositeChangeToken(tokens);
    }

    private static bool MatchesPattern(string fileName, string pattern)
    {
        // Simple glob matching - in production, consider using a proper glob library
        if (pattern == "*") return true;
        if (pattern.StartsWith("*") && pattern.EndsWith("*"))
        {
            var search = pattern.Trim('*');
            return fileName.Contains(search);
        }
        if (pattern.StartsWith("*"))
        {
            var extension = pattern.TrimStart('*');
            return fileName.EndsWith(extension);
        }
        return fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }
}
```

### Testing File Providers

```csharp
using Microsoft.Extensions.FileProviders;
using Moq;

public class TemplateServiceTests
{
    [Fact]
    public async Task GetTemplateContentAsync_ReturnsContent_WhenTemplateExists()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockFileInfo = new Mock<IFileInfo>();

        var expectedContent = "<html><body>Hello World</body></html>";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContent));

        mockFileInfo.Setup(f => f.Exists).Returns(true);
        mockFileInfo.Setup(f => f.CreateReadStream()).Returns(stream);

        mockFileProvider
            .Setup(p => p.GetFileInfo("Templates/test.html"))
            .Returns(mockFileInfo.Object);

        var service = new TemplateService(mockFileProvider.Object, Mock.Of<ILogger<TemplateService>>());

        // Act
        var result = await service.GetTemplateContentAsync("test");

        // Assert
        Assert.Equal(expectedContent, result);
    }

    [Fact]
    public void GetAvailableTemplates_ReturnsHtmlFiles()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockDirectoryContents = new Mock<IDirectoryContents>();

        var files = new[]
        {
            CreateMockFileInfo("template1.html", false),
            CreateMockFileInfo("template2.cshtml", false),
            CreateMockFileInfo("data.json", false),
            CreateMockFileInfo("styles", true) // directory
        };

        mockDirectoryContents.Setup(d => d.Exists).Returns(true);
        mockDirectoryContents.Setup(d => d.GetEnumerator()).Returns(files.Select(f => f.Object).GetEnumerator());

        mockFileProvider
            .Setup(p => p.GetDirectoryContents("Templates"))
            .Returns(mockDirectoryContents.Object);

        var service = new TemplateService(mockFileProvider.Object, Mock.Of<ILogger<TemplateService>>());

        // Act
        var result = service.GetAvailableTemplates().ToList();

        // Assert
        Assert.Contains("template1", result);
        Assert.DoesNotContain("template2", result);
        Assert.DoesNotContain("data", result);
        Assert.DoesNotContain("styles", result);
    }

    private static Mock<IFileInfo> CreateMockFileInfo(string name, bool isDirectory)
    {
        var mock = new Mock<IFileInfo>();
        mock.Setup(f => f.Name).Returns(name);
        mock.Setup(f => f.IsDirectory).Returns(isDirectory);
        return mock;
    }
}
```

## Configuration

### Program.cs Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure physical file provider for content root
builder.Services.AddSingleton<IFileProvider>(builder.Environment.ContentRootFileProvider);

// Configure web root file provider
builder.Services.AddSingleton<IFileProvider>("WebRoot", builder.Environment.WebRootFileProvider);

// Configure custom file providers
var mediaPath = Path.Combine(builder.Environment.ContentRootPath, "Media");
builder.Services.AddSingleton<IFileProvider>("Media", new PhysicalFileProvider(mediaPath));

// Configure embedded file providers
builder.Services.AddSingleton<IFileProvider>("EmbeddedViews",
    new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "Views"));

builder.Services.AddSingleton<IFileProvider>("EmbeddedResources",
    new ManifestEmbeddedFileProvider(typeof(Program).Assembly));

// Configure composite file provider
var physicalProvider = new PhysicalFileProvider(builder.Environment.ContentRootPath);
var embeddedProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly);
var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
builder.Services.AddSingleton<IFileProvider>(compositeProvider);

var app = builder.Build();
```

## Advanced Patterns

### File Provider with Caching

```csharp
public class CachedFileProvider : IFileProvider
{
    private readonly IFileProvider _innerProvider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedFileProvider> _logger;

    public CachedFileProvider(
        IFileProvider innerProvider,
        IMemoryCache cache,
        ILogger<CachedFileProvider> logger)
    {
        _innerProvider = innerProvider;
        _cache = cache;
        _logger = logger;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var cacheKey = $"fileinfo:{subpath}";
        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            var fileInfo = _innerProvider.GetFileInfo(subpath);

            // Set up change monitoring to invalidate cache
            var changeToken = _innerProvider.Watch(subpath);
            entry.AddExpirationToken(changeToken);

            return fileInfo;
        });
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var cacheKey = $"dircontents:{subpath}";
        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            var contents = _innerProvider.GetDirectoryContents(subpath);

            // Watch for directory changes
            var changeToken = _innerProvider.Watch(Path.Combine(subpath, "*"));
            entry.AddExpirationToken(changeToken);

            return contents;
        });
    }

    public IChangeToken Watch(string filter)
    {
        return _innerProvider.Watch(filter);
    }
}
```

### File Provider for Multi-Tenant Applications

```csharp
public class MultitenantFileProvider : IFileProvider
{
    private readonly IFileProvider _baseProvider;
    private readonly ITenantContext _tenantContext;

    public MultitenantFileProvider(IFileProvider baseProvider, ITenantContext tenantContext)
    {
        _baseProvider = baseProvider;
        _tenantContext = tenantContext;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var tenantPath = GetTenantPath(subpath);
        return _baseProvider.GetFileInfo(tenantPath);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var tenantPath = GetTenantPath(subpath);
        return _baseProvider.GetDirectoryContents(tenantPath);
    }

    public IChangeToken Watch(string filter)
    {
        var tenantFilter = GetTenantPath(filter);
        return _baseProvider.Watch(tenantFilter);
    }

    private string GetTenantPath(string path)
    {
        var tenantId = _tenantContext.Tenant?.Id ?? "default";
        return Path.Combine("tenants", tenantId, path.TrimStart('/'));
    }
}
```

### File Provider with Access Control

```csharp
public class AccessControlledFileProvider : IFileProvider
{
    private readonly IFileProvider _innerProvider;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccessControlledFileProvider(
        IFileProvider innerProvider,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _innerProvider = innerProvider;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (!CanAccessFile(subpath))
        {
            return new NotFoundFileInfo(subpath);
        }

        return _innerProvider.GetFileInfo(subpath);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        if (!CanAccessDirectory(subpath))
        {
            return new NotFoundDirectoryContents();
        }

        return _innerProvider.GetDirectoryContents(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        // Only allow watching if user has access to the pattern
        if (!CanAccessPattern(filter))
        {
            return NullChangeToken.Singleton;
        }

        return _innerProvider.Watch(filter);
    }

    private bool CanAccessFile(string path)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return false;

        // Implement authorization logic based on path and user
        var authorizationResult = _authorizationService.AuthorizeAsync(
            user, path, "FileAccess").GetAwaiter().GetResult();

        return authorizationResult.Succeeded;
    }

    private bool CanAccessDirectory(string path) => CanAccessFile(path);

    private bool CanAccessPattern(string pattern) => true; // Implement pattern-based authorization
}
```

## Performance Considerations

- **Caching**: Cache file metadata and content when appropriate
- **Change Tokens**: Use efficient change monitoring instead of polling
- **Streaming**: Use CreateReadStream() for large files to avoid memory pressure
- **Directory Enumeration**: Be cautious with deep directory structures
- **Provider Selection**: Choose the most efficient provider for your use case

## Security Considerations

- **Path Traversal**: Validate and sanitize file paths to prevent directory traversal attacks
- **Access Control**: Implement proper authorization for file access
- **Resource Limits**: Set limits on file sizes and concurrent access
- **Embedded Resources**: Be aware that embedded files are accessible to anyone with assembly access
- **Change Monitoring**: Secure file watching to prevent information disclosure

## Related Libraries

- **Microsoft.Extensions.FileProviders.Physical**: Physical file system access
- **Microsoft.Extensions.FileProviders.Embedded**: Embedded resource access
- **Microsoft.Extensions.FileProviders.Composite**: Multiple provider composition
- **Microsoft.Extensions.Primitives**: Change tokens and common types
- **Microsoft.Extensions.Caching.Memory**: File content caching

This library forms the foundation of B2X's file access layer, enabling consistent, secure, and performant file operations across the distributed multi-tenant architecture.