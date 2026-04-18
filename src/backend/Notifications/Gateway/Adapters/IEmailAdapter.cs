using System.Threading.Tasks;

namespace B2X.Notifications.Adapters
{
    public interface IEmailAdapter
    {
        Task SendAsync(string tenantId, object? payload);
    }
}
