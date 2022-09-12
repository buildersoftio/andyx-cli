using Buildersoft.Andy.X.Model.Entities.Core.Tenants;

namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentRetention
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public RetentionType Type { get; set; }
        public long TimeToLiveInMinutes { get; set; }
    }
}
