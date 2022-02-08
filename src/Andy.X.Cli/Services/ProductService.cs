using Andy.X.Cli.Models;
using ConsoleTables;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class ProductService
    {
        public static void GetProducts(string tenant)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "PRODUCT_NAME");
                    //List<string> list = content.JsonToObject<List<string>>();
                    List<Product> list = JsonConvert.DeserializeObject<List<Product>>(content);

                    int k = 0;
                    foreach (var item in list)
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

        public static void GetProduct(string tenant, string product)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/products/{product}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "PRODUCT_NAME", "COMPONENTS");
                    var productDetail = JsonConvert.DeserializeObject<Product>(content);
                    table.AddRow(productDetail.Id, productDetail.Name, productDetail.Components.Count);
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
