namespace B2X.CMS.Core.Domain
{
    /// <summary>
    /// Base class for domain aggregate roots
    /// </summary>
    public abstract class AggregateRoot
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
