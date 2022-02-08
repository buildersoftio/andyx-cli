using Andy.X.Cli.Models;
using ConsoleTables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andy.X.Cli.Services
{
    public static class ProducerService
    {
        public static void GetProducers()
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/producers";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "PRODUCER_ID");
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

        public static void GetProducer(string producer)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/producers/{producer}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "TOPIC", "ID", "PRODUCER_NAME");
                    var producerDetail = JsonConvert.DeserializeObject<Producer>(content);
                    table.AddRow(producerDetail.Tenant, producerDetail.Product, producerDetail.Component, producerDetail.Tenant, producerDetail.Id, producerDetail.ProducerName);
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
