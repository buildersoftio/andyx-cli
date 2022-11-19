using Buildersoft.Andy.X.Model.Entities.Core.Tenants;

namespace Buildersoft.Andy.X.Model.Entities.Core.Products
{
    public class ProductRetention
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public RetentionType Type { get; set; }
        public long TimeToLiveInMinutes { get; set; }
    }
}
