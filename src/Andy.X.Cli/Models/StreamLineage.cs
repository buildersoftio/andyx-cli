namespace Andy.X.Cli.Models
{
    public class StreamLineage
    {
        public List<Producer> Producers { get; set; }
        public string Topic { get; set; }
        public string TopicPhysicalPath { get; set; }
        public List<Consumer> Consumers { get; set; }

        public StreamLineage()
        {
            Producers = new List<Producer>();
            Consumers = new List<Consumer>();
        }
    }
}
