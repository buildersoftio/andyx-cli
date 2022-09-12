namespace Buildersoft.Andy.X.Model.Entities.Core.Tenants
{
    public class TenantToken
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Secret { get; set; }

        public List<TenantTokenRole> Roles { get; set; }

        public DateTimeOffset ExpireDate { get; set; }


        public DateTimeOffset IssuedDate { get; set; }
    }

    public enum TenantTokenRole
    {
        Produce,
        Consume
    }

}
