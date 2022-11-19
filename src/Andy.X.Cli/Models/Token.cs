namespace Andy.X.Cli.Models
{
    public class Token
    {
        public Guid Key { get; set; }
        public string Description { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
        public bool IsActive { get; set; }
    }
}
