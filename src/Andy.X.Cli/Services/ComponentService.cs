using Andy.X.Cli.Models;
using Andy.X.Cli.Models.Configurations;
using Andy.X.Cli.Utilities.Extensions;
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
                client.AddBasicAuthorizationHeader(node);

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
                client.AddBasicAuthorizationHeader(node);

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
                client.AddBasicAuthorizationHeader(node);

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

        public static void PostComponentToken(string tenant, string product, string component, ComponentToken componentToken)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/tokens";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

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

        public static void DeleteComponentToken(string tenant, string product, string component, string token)
        {
            var node = NodeService.GetNode();
            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/tokens/{token}/revoke";

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.DeleteAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "TENANT", "PRODUCT", "COMPONENT", "TOKEN", "RESULT");
                    //List<string> list = content.JsonToObject<List<string>>();
                    table.AddRow("1", tenant, product, component, token, content);
                    table.Write();
                }
                else
                {
                    var table = new ConsoleTable("ID", "RESULT");
                    table.AddRow("1", content);
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

        public static void PostComponentRetention(string tenant, string product, string component, ComponentRetention retention)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/retention";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                var settings = JsonConvert.SerializeObject(retention);
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

        public static void GetComponentRetention(string tenant, string product, string component)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}/components/{component}/retention";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "NAME", "RETENTION_TIME_IN_MINUTES", "STATUS");
                    var result = JsonConvert.DeserializeObject<ComponentRetention>(content);

                    table.AddRow("1", result.Name, result.RetentionTimeInMinutes, "ACTIVE");
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
