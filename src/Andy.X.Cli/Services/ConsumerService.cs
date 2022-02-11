using Andy.X.Cli.Models;
using Andy.X.Cli.Utilities.Extensions;
using ConsoleTables;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class ConsumerService
    {
        public static void GetConsumers()
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/consumers";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "CONSUMER_ID");
                    //List<string> list = content.JsonToObject<List<string>>();
                    List<string> list = JsonConvert.DeserializeObject<List<string>>(content);

                    int k = 0;
                    foreach (var item in list)
                    {
                        k++;
                        table.AddRow(k, item);
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

        public static void GetConsumer(string consumer)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/consumers/{consumer}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "TOPIC", "ID", "CONSUMER_NAME", "CONNECTIONS", "CURRENT_CONNECTION_INDEX", "SUBSCRIPTION_TYPE", "INITIAL_POSITION");
                    var consumerDetail = JsonConvert.DeserializeObject<Consumer>(content);
                    table.AddRow(consumerDetail.Tenant, consumerDetail.Product, consumerDetail.Component, consumerDetail.Tenant, consumerDetail.Id, consumerDetail.ConsumerName,
                        consumerDetail.Connections.Count, consumerDetail.CurrentConnectionIndex, consumerDetail.SubscriptionType, consumerDetail.ConsumerSettings.InitialPosition);
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
