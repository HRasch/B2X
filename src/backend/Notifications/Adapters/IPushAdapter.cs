using System.Threading.Tasks;

namespace B2X.Notifications.Adapters
{
    public interface IPushAdapter
    {
        Task SendAsync(string tenantId, object? payload);
    }
}
