using System.Collections.Generic;
using System.Threading;

namespace B2Connect.Services.Search.Services;

public interface ICatalogProductProvider
{
    Task<(IEnumerable<dynamic> Items, int Total)> GetPageAsync(int page, int pageSize, CancellationToken ct = default);
}
