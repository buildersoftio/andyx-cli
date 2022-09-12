using Andy.X.Cli.Models.Subscriptions;
using Andy.X.Cli.Utilities;
using Andy.X.Cli.Utilities.Extensions;
using Buildersoft.Andy.X.Model.Entities.Core.Subscriptions;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using ConsoleTables;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;

namespace Andy.X.Cli.Services
{
    public static class SubscriptionService
    {
        public static void GetSubscriptions(string tenant, string product, string component, string topic)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/subscriptions";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "TOPIC", "SUBSCRIPTION_NAME", "SUBSCRIPTION_TYPE");
                    List<SubscriptionName> list = JsonConvert.DeserializeObject<List<SubscriptionName>>(content)!;

                    foreach (var item in list)
                    {
                        table.AddRow(tenant, product, component, topic, item.Name, item.Type.ToString());
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
        public static void GetSubscription(string tenant, string product, string component, string topic, string subscription)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/subscriptions/{subscription}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "NAME", "TYPE", "MODE", "INITIAL_POSITION", "PUBLIC_IP_RANGE", "PRIVATE_IP_RANGE" ,"UPDATED_DATE", "CREATED_DATE", "UPDATED_BY", "CREATED_BY");

                    var topicDetails = JsonConvert.DeserializeObject<Subscription>(content);
                    table.AddRow(topicDetails!.Id, topicDetails.Name, topicDetails.SubscriptionType.ToString(), 
                        topicDetails.SubscriptionMode.ToString(), topicDetails.InitialPosition.ToString(), 
                        string.Join(",", topicDetails.PublicIpRange),
                        string.Join(",", topicDetails.PrivateIpRange),
                        topicDetails.UpdatedDate, topicDetails.CreatedDate, topicDetails.UpdatedBy, topicDetails.CreatedBy);
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

        public static void PostSubscription(string tenant, string product, string component, string topic, string subscription, SubscriptionType subscriptionType, SubscriptionMode subscriptionMode, InitialPosition initialPosition)
        {
            var node = NodeService.GetNode();

            var query = new Dictionary<string, string>
            {
                ["subscriptionType"] = subscriptionType.ToString(),
                ["subscriptionMode"] = subscriptionMode.ToString(),
                ["initialPosition"] = initialPosition.ToString()
            };

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/subscriptions/{subscription}";
            var stringUri = QueryHelpers.AddQueryString(request, query);

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.PostAsync(stringUri, null).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Subscription '{subscription}' at '{tenant}/{product}/{component}/{topic}' has been created succesfully!");
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
