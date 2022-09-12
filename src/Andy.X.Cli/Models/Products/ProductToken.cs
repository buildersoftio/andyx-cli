namespace Buildersoft.Andy.X.Model.Entities.Core.Products
{
    public class ProductToken
    {
     
        public Guid Id { get; set; }

        public string Secret { get; set; }

        public List<ProductTokenRole> Roles { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset ExpireDate { get; set; }

        public string Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }
    }

    public enum ProductTokenRole
    {
        Produce,
        Consume
    }
}
