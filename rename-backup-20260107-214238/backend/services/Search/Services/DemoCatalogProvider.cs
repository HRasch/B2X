using B2X.Catalog.Endpoints.Dev;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.Services.Search.Services;

public class DemoCatalogProvider : ICatalogProductProvider
{
    public Task<(IEnumerable<dynamic> Items, int Total)> GetPageAsync(int page, int pageSize, CancellationToken ct = default)
    {
        DemoProductStore.EnsureInitialized();
        var (items, total) = DemoProductStore.GetPage(page, pageSize);
        return Task.FromResult((items, total));
    }
}
