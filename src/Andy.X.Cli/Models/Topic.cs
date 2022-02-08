namespace Andy.X.Cli.Models
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public TopicSettings TopicSettings { get; set; }

        public Topic()
        {
            TopicSettings = new TopicSettings();
        }
    }

    public class TopicSettings
    {
        public bool IsPersistent { get; set; }

        public TopicSettings()
        {
            IsPersistent = true;
        }
    }
}
