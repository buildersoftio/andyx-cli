using Andy.X.Cli.Models;
using Andy.X.Cli.Utilities.Extensions;
using ConsoleTables;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class StreamLineageService
    {
        public static void GetStreamLineage(string tenant)
        {
            string req = $"api/v1/tenants/{tenant}/lineage";
            RequestStreamLineage(req);
        }
        public static void GetStreamLineage(string tenant, string product)
        {
            string req = $"api/v1/tenants/{tenant}/products/{product}/lineage";
            RequestStreamLineage(req);
        }
        public static void GetStreamLineage(string tenant, string product, string component)
        {
            string req = $"api/v1/tenants/{tenant}/products/{product}/components/{component}/lineage";
            RequestStreamLineage(req);
        }

        public static void GetStreamLineage(string tenant, string product, string component, string topic)
        {
            string req = $"api/v1/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/lineage";
            RequestStreamLineage(req, true);
        }

        private static void RequestStreamLineage(string requestPath, bool isTopic = false)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}{requestPath}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (isTopic == false)
                    {
                        List<StreamLineage>? list = JsonConvert.DeserializeObject<List<StreamLineage>>(content);
                        DrawStreamLineage(list!);
                    }
                    else
                    {
                        StreamLineage? stream = JsonConvert.DeserializeObject<StreamLineage>(content);
                        DrawStreamLineage(new List<StreamLineage>() { stream! });
                    }
                }
                else
                {
                    var table = new ConsoleTable("STATUS", "ERROR");
                    table.AddRow(httpResponseMessage.StatusCode.ToString(), content);
                    table.Write();
                }
            }
            catch (Exception)
            {
                var table = new ConsoleTable("STATUS", "ERROR");

                table.AddRow("NOT_CONNECTED", "It can not connect to the node, check network connectivity");
                table.Write();
            }
        }

        private static void DrawStreamLineage(List<StreamLineage> streamLineages)
        {
            int k = 1;
            foreach (var streamLineage in streamLineages)
            {
                Console.WriteLine("");
                Console.WriteLine($"Stream Lineage {k}");

                string producerNames = string.Join(", ", streamLineage.Producers.Select(x => x.ProducerName));
                string consumerNames = string.Join(", ", streamLineage.Consumers.Select(x => x.ConsumerName));

                Console.Write("PRODUCERS: ");
                if (producerNames != "")
                    Console.Write(producerNames);
                else
                    Console.Write("NO_PRODUCER_CONNECTED");
                Console.WriteLine($"  ----->>>");
                Console.WriteLine($"    TOPIC: {streamLineage.Topic} ({streamLineage.TopicPhysicalPath})");
                Console.Write("CONSUMERS: ");
                Console.Write("----->>>  ");
                if (consumerNames != "")
                    Console.WriteLine(consumerNames);
                else
                    Console.WriteLine("NO_CONSUMER_CONNECTED");

                k++;
            }
        }

    }
}
