using Andy.X.Cli.Models;
using Andy.X.Cli.Utilities.Extensions;
using Andy.X.Cli.Utilities;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using ConsoleTables;
using Newtonsoft.Json;
using System.Text;
using Buildersoft.Andy.X.Model.Entities.Core.Products;

namespace Andy.X.Cli.Services
{
    public static class ProductTokenService
    {
        public static void GetProductTokens(string tenant, string product)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/tokens";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "KEY", "DESCRIPTION", "EXPIRE_DATE", "IS_ACTIVE");
                    List<Token> list = JsonConvert.DeserializeObject<List<Token>>(content)!;

                    foreach (var item in list)
                    {
                        table.AddRow(tenant, product, item.Key, item.Description, item.ExpireDate, item.IsActive);
                    }
                    table.Write();
                }
            }
            catch (Exception ex)
            {
                var table = new ConsoleTable("STATUS", "ERROR");

                table.AddRow("NOT_CONNECTED", $"It can not connect to the node, check network connectivity, {ex.Message}");
                table.Write();
            }

        }
        public static void GetProductToken(string tenant, string product, Guid key)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/tokens/{key}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("KEY", "DESCRIPTION", "IS_ACTIVE", "SECRET", "ROLES", "EXPIRE_DATE", "ISSUED_DATE");

                    var productTokenDetails = JsonConvert.DeserializeObject<ProductToken>(content);
                    table.AddRow(productTokenDetails!.Id, productTokenDetails.Description, productTokenDetails.IsActive,
                        "********************",
                        string.Join(",", productTokenDetails.Roles),
                        productTokenDetails.ExpireDate,
                        productTokenDetails.IssuedDate);
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

        public static void PostProductToken(string tenant, string product, ProductToken tenantToken)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/tokens";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                var settings = JsonConvert.SerializeObject(tenantToken);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PostAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var tenantResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                    Console.WriteLine("");
                    Console.WriteLine($"Product token has been created succesfully!");
                    Console.WriteLine($"Key '{tenantResponse!.Key}'");
                    Console.WriteLine($"Secret '{tenantResponse.Secret}'");
                    Console.WriteLine($"-----------------------------------------------------------------------");
                    Console.WriteLine($"Please! Make sure to store the secret, you can not get this without re-creating a new token.");
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

        public static void RevokeProductToken(string tenant, string product, Guid key)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/tokens/{key}/revoke";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.PutAsync(request, null).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Token has been revoked!");
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
