using Andy.X.Cli.Models;
using Andy.X.Cli.Utilities.Extensions;
using ConsoleTables;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class TopicService
    {
        public static void GetTopics(string tenant, string product, string component)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/topics";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "ID", "TOPIC_NAME");
                    List<Topic> listResult = JsonConvert.DeserializeObject<List<Topic>>(content);

                    foreach (var item in listResult)
                    {
                        table.AddRow(tenant, product, component, item.Id, item.Name);
                    }
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

        public static void GetTopic(string tenant, string product, string component, string topic)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "ID", "TOPIC_NAME", "IS_PERSISTENT");
                    var productDetail = JsonConvert.DeserializeObject<Topic>(content);
                    table.AddRow(tenant, product, component, productDetail.Id, productDetail.Name);
                    table.Write();
                }
                else
                {
                    var table = new ConsoleTable("STATUS", "ERROR");

                    table.AddRow(httpResponseMessage.StatusCode, content);
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

    }
}
