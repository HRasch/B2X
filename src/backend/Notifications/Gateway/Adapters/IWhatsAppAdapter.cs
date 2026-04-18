using System.Threading.Tasks;

namespace B2X.Notifications.Adapters
{
    public interface IWhatsAppAdapter
    {
        Task SendAsync(string tenantId, object? payload);
    }
}
