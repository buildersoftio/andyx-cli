using Andy.X.Cli.Models.Configurations;
using ConsoleTables;
using Newtonsoft.Json;
using System.Text;

namespace Andy.X.Cli.Services
{
    public static class TenantService
    {
        public static void GetTenants()
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants";
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
                    var table = new ConsoleTable("ID", "TENANT_NAME");
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

        public static void GetTenant(string tenant)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}";
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
                    var table = new ConsoleTable("NAME", "ALLOW_PRODUCT_CREATION", "ENABLE_ENCRYPTION", "ENABLE_GEOREPLICATION", "LOGGING", "ENABLE_AUTHORIZATION", "DIGITAL_SIGNATURE", "CERTIFICATE_PATH");
                    var tenantDetail = JsonConvert.DeserializeObject<TenantConfiguration>(content);
                    table.AddRow(tenantDetail.Name, tenantDetail.Settings.AllowProductCreation, tenantDetail.Settings.EnableEncryption, tenantDetail.Settings.EnableGeoReplication,
                        tenantDetail.Settings.Logging, tenantDetail.Settings.EnableAuthorization, tenantDetail.Settings.DigitalSignature, tenantDetail.Settings.CertificatePath);
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

        public static void PostTenant(string tenant, TenantSettings tenantSettings)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.DefaultRequestHeaders.Add("x-andyx-node-username", node.Username);
                client.DefaultRequestHeaders.Add("x-andyx-node-password", node.Password);

                var settings = JsonConvert.SerializeObject(tenantSettings);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PostAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Tenant with name '{tenant}' has been created succesfully!");
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

        public static void PostTenantToken(string tenant, DateTime expireDate)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/tokens";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");
                client.DefaultRequestHeaders.Add("x-andyx-node-username", node.Username);
                client.DefaultRequestHeaders.Add("x-andyx-node-password", node.Password);

                var settings = JsonConvert.SerializeObject(expireDate);
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

        public static void GetTenantTokens(string tenant)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/tenants/{tenant}/tokens";
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
                    var table = new ConsoleTable("ID", "TENANT", "TOKEN", "IS_ACTIVE", "EXPIRE_DATE", "ISSUED_FOR", "ISSUED_DATE");
                    //List<string> list = content.JsonToObject<List<string>>();
                    List<TenantToken> list = JsonConvert.DeserializeObject<List<TenantToken>>(content);

                    int k = 0;
                    foreach (var item in list)
                    {
                        k++;
                        table.AddRow(k, tenant, item.Token, item.IsActive, item.ExpireDate, item.IssuedFor, item.IssuedDate);
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
    }
}
