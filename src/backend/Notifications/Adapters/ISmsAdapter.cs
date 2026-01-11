using System.Threading.Tasks;

namespace B2X.Notifications.Adapters
{
    public interface ISmsAdapter
    {
        Task SendAsync(string tenantId, object? payload);
    }
}
