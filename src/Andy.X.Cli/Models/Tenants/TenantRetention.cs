namespace Buildersoft.Andy.X.Model.Entities.Core.Tenants
{
    public class TenantRetention
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public RetentionType Type { get; set; }
        public long TimeToLiveInMinutes { get; set; }
    }

    public enum RetentionType
    {
        SOFT_TTL,
        HARD_TTL
    }
}
