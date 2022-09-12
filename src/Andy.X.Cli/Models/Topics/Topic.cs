namespace Buildersoft.Andy.X.Model.Entities.Core.Topics
{
    public class Topic
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
