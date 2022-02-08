using Andy.X.Cli.Models.Configurations;
using System.Collections.Concurrent;

namespace Andy.X.Cli.Models
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ComponentSettings Settings { get; set; }
        public ConcurrentDictionary<string, Topic> Topics { get; set; }

        public Component()
        {
            Settings = new ComponentSettings();
            Topics = new ConcurrentDictionary<string, Topic>();
        }
    }
}
