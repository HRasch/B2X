namespace B2X.Email.Models;

/// <summary>
/// Email-Statistiken
/// </summary>
public class EmailStatistics
{
    public int TotalSent { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalOpened { get; set; }
    public int TotalClicked { get; set; }
    public int TotalBounced { get; set; }
    public int TotalFailed { get; set; }
    public double OpenRate => TotalSent > 0 ? (double)TotalOpened / TotalSent * 100 : 0;
    public double ClickRate => TotalSent > 0 ? (double)TotalClicked / TotalSent * 100 : 0;
    public double BounceRate => TotalSent > 0 ? (double)TotalBounced / TotalSent * 100 : 0;
    public double DeliveryRate => TotalSent > 0 ? (double)TotalDelivered / TotalSent * 100 : 0;
}
