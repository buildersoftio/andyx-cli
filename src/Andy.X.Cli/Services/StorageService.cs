using Andy.X.Cli.Models;
using Andy.X.Cli.Utilities.Extensions;
using ConsoleTables;
using Newtonsoft.Json;

namespace Andy.X.Cli.Services
{
    public static class StorageService
    {
        public static void GetStorages()
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/storages";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "STORAGE_NAME");
                    List<string> list = JsonConvert.DeserializeObject<List<string>>(content);

                    int k = 0;
                    foreach (var item in list)
                    {
                        k++;
                        table.AddRow(k, item);
                    }
                    table.Write();
                }
                else
                {
                    var table = new ConsoleTable("STATUS", "ERROR");
                    table.AddRow(httpResponseMessage.StatusCode.ToString(), content);
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

        public static void GetStorageDetails(string storageName)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v1/storages/{storageName}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", $"Andy X Cli");

                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("KEY", "VALUE");
                    var storage = JsonConvert.DeserializeObject<Storage>(content);

                    table.AddRow("ID", storage.StorageId);
                    table.AddRow("NAME", storage.StorageName);
                    table.AddRow("TYPE_OF_CONNECTION", "SHARDING");
                    table.AddRow("STATUS", storage.StorageStatus.ToString());

                    int k = 0;
                    foreach (var agent in storage.Agents)
                    {
                        table.AddRow($"AGENT[{k}]", $"{agent.Value.AgentName} | {agent.Value.ConnectionId}");
                        k++;
                    }

                    table.AddRow("AGENT_CURRENT_INDEX", storage.ActiveAgentIndex);
                    table.AddRow("IS_LOAD_BALANCED", storage.IsLoadBalanced);
                    table.AddRow("AGENT_MAX_NUMBER", storage.AgentMaxNumber);
                    table.AddRow("AGENT_MIN_NUMBER", storage.AgentMinNumber);

                    //table.AddRow(storage.StorageId, storage.StorageName, storage.StorageStatus, storage.Agents.Count, storage.ActiveAgentIndex, storage.IsLoadBalanced, storage.AgentMaxNumber, storage.AgentMinNumber);

                    table.Write();
                }
                else
                {
                    var table = new ConsoleTable("STATUS", "ERROR");
                    table.AddRow(httpResponseMessage.StatusCode.ToString(), content);
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
