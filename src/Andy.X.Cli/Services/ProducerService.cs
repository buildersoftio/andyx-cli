﻿using Andy.X.Cli.Utilities;
using Andy.X.Cli.Utilities.Extensions;
using Buildersoft.Andy.X.Model.Entities.Core.Producers;
using ConsoleTables;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class ProducerService
    {
        public static void GetProducers(string tenant, string product, string component, string topic)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/producers";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "TOPIC", "PRODUCER_NAME");
                    List<string> list = JsonConvert.DeserializeObject<List<string>>(content)!;

                    foreach (var item in list)
                    {
                        table.AddRow(tenant, product, component, topic, item);
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
        public static void GetProducer(string tenant, string product, string component, string topic, string producer)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/producers/{producer}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "NAME", "DESCRIPTION", "INSTANCE_TYPE", "PUBLIC_IP_RANGE", "PRIVATE_IP_RANGE", "UPDATED_DATE", "CREATED_DATE", "UPDATED_BY", "CREATED_BY");

                    var producerDetails = JsonConvert.DeserializeObject<Producer>(content);
                    table.AddRow(producerDetails!.Id, producerDetails.Name, producerDetails.Description,
                        producerDetails.InstanceType.ToString(),
                        string.Join(",", producerDetails.PublicIpRange),
                        string.Join(",", producerDetails.PrivateIpRange),
                        producerDetails.UpdatedDate, producerDetails.CreatedDate, producerDetails.UpdatedBy, producerDetails.CreatedBy);
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

        public static void PostProducer(string tenant, string product, string component, string topic, string producer, string description, ProducerInstanceType producerInstanceType)
        {
            var node = NodeService.GetNode();

            var query = new Dictionary<string, string>
            {
                ["description"] = description,
                ["instanceType"] = producerInstanceType.ToString(),
            };

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/producers/{producer}";
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
                    Console.WriteLine($"Producer '{producer}' at '{tenant}/{product}/{component}/{topic}' has been created succesfully!");
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
