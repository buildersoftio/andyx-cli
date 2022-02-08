using System.Collections.Concurrent;

namespace Andy.X.Cli.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ConcurrentDictionary<string, Component> Components { get; set; }


        public Product()
        {
            Components = new ConcurrentDictionary<string, Component>();
        }
    }
}
