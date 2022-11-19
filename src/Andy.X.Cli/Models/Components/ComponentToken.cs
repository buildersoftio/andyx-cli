namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentToken
    {
        public Guid Id { get; set; }


        public string Secret { get; set; }

        public List<ComponentTokenRole> Roles { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset ExpireDate { get; set; }

        public string IssuedFor { get; set; }
        public string Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }
    }

    public enum ComponentTokenRole
    {
        Produce,
        Consume
    }
}
