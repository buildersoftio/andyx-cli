using Andy.X.Cli.Models;
using Andy.X.Cli.Models.Configurations;
using ConsoleTables;
using Newtonsoft.Json;
using System.Text;

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
                client.DefaultRequestHeaders.Add("x-andyx-node-username", node.Username);
                client.DefaultRequestHeaders.Add("x-andyx-node-password", node.Password);

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
                client.DefaultRequestHeaders.Add("x-andyx-node-username", node.Username);
                client.DefaultRequestHeaders.Add("x-andyx-node-password", node.Password);

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

        public static void GetComponentTokens(string tenant, string product, string component)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/tokens";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.DefaultRequestHeaders.Add("x-andyx-node-username", node.Username);
                client.DefaultRequestHeaders.Add("x-andyx-node-password", node.Password);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "NAME", "DESCRIPTION", "TOKEN", "IS_ACTIVE", "CAN_CONSUME", "CAN_PRODUCE", "EXPIRE_DATE", "ISSUED_FOR", "ISSUED_DATE");
                    List<ComponentToken> listResult = JsonConvert.DeserializeObject<List<ComponentToken>>(content);

                    int k = 0;
                    foreach (var item in listResult)
                    {
                        k++;
                        table.AddRow(k, item.Name, item.Description, item.Token, item.IsActive, item.CanConsume, item.CanProduce, item.ExpireDate, item.IssuedFor, item.IssuedDate);
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

        public static void PostComponentToken(string tenant, string product, string component,ComponentToken componentToken)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/tokens";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.DefaultRequestHeaders.Add("x-andyx-node-username", node.Username);
                client.DefaultRequestHeaders.Add("x-andyx-node-password", node.Password);

                var settings = JsonConvert.SerializeObject(componentToken);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PostAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"{content}");
                    Console.WriteLine($"-----------------------------------------------------------------------");
                    Console.WriteLine("");
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
