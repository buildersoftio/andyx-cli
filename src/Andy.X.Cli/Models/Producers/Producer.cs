namespace Buildersoft.Andy.X.Model.Entities.Core.Producers
{
    public class Producer
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ProducerInstanceType InstanceType { get; set; }

        public List<string> PublicIpRange { get; set; }
        public List<string> PrivateIpRange { get; set; }


        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }

    public enum ProducerInstanceType
    {
        Single,
        Multiple
    }
}
