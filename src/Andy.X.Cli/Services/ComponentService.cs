using Andy.X.Cli.Models;
using ConsoleTables;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class ComponentService
    {
        public static void GetComponents(string tenant, string product)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "COMPONENT_NAME");
                    List<Component> listResult = JsonConvert.DeserializeObject<List<Component>>(content);

                    int k = 0;
                    foreach (var item in listResult)
                    {
                        k++;
                        table.AddRow(item.Id, item.Name);
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

        public static void GetComponent(string tenant, string product, string component)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "COMPONENT_NAME", "TOPICS", "ALLOW_SCHEMA_VALIDATION", "ALLOW_TOPIC_CREATION", "ENABLE_AUTHORIZATION", "TOKENS");
                    var productDetail = JsonConvert.DeserializeObject<Component>(content);
                    table.AddRow(productDetail.Id, productDetail.Name, productDetail.Topics.Count, productDetail.Settings.AllowSchemaValidation, productDetail.Settings.AllowTopicCreation, productDetail.Settings.EnableAuthorization, productDetail.Settings.Tokens.Count);
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
