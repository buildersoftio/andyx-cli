using Andy.X.Cli.Utilities.Extensions;
using Andy.X.Cli.Utilities;
using ConsoleTables;
using Newtonsoft.Json;
using System.Text;
using Buildersoft.Andy.X.Model.Entities.Core.Products;

namespace Andy.X.Cli.Services
{
    public static class ProductRetentionService
    {
        public static void GetProductRetentions(string tenant, string product)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/retentions";

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "ID", "NAME", "TYPE", "TTL (in minutes)");
                    List<ProductRetention> list = JsonConvert.DeserializeObject<List<ProductRetention>>(content)!;

                    foreach (var item in list)
                    {
                        table.AddRow(tenant, item.Id, item.Name, item.Type.ToString(), item.TimeToLiveInMinutes);
                    }
                    table.Write();
                }
            }
            catch (Exception)
            {
                var table = new ConsoleTable("STATUS", "ERROR");

                table.AddRow("NOT_CONNECTED", $"It can not connect to the node, check network connectivity");
                table.Write();
            }

        }

        public static void PostProductRetention(string tenant, string product, ProductRetention productRetention)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/retentions";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                var settings = JsonConvert.SerializeObject(productRetention);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PostAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Product retention has been created succesfully! This is async process, it will take some time to start reflecting");
                    Console.WriteLine($"-----------------------------------------------------------------------------------------------------------------");
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

                table.AddRow("NOT_CONNECTED", $"It can not connect to the node, check network connectivity");
                table.Write();
            }

        }

        public static void UpdateProductRetention(string tenant, string product, long id, ProductRetention productRetention)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/retentions/{id}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                var settings = JsonConvert.SerializeObject(productRetention);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PutAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Product retention has been updated succesfully! This is async process, it will take some time to start reflecting");
                    Console.WriteLine($"-----------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");
                }
                else
                {
                    var table = new ConsoleTable("STATUS", "ERROR");

                    table.AddRow(httpResponseMessage.StatusCode, content);
                    table.Write();
                }
            }
            catch (Exception ex)
            {
                var table = new ConsoleTable("STATUS", "ERROR");

                table.AddRow("NOT_CONNECTED", $"It can not connect to the node, check network connectivity. Details {ex.Message}");
                table.Write();
            }

        }
        public static void DeleteProductRetention(string tenant, string product, long id)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/retentions/{id}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.DeleteAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Product retention has been deleted succesfully! This is async process, it will take some time to start reflecting");
                    Console.WriteLine($"-----------------------------------------------------------------------------------------------------------------");
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
