using Andy.X.Cli.Utilities.Extensions;
using Andy.X.Cli.Utilities;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using ConsoleTables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andy.X.Cli.Models.Clusters;

namespace Andy.X.Cli.Services
{
    public static class ClusterService
    {
        public static void GetClusterConfiguration()
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/clusters";

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("NAME", "STATUS", "DISTRIBUTION_TYPE", "IN_THROUGHPUT_IN_MB", "OUT_THROUGHPUT_IN_MB", "ACTIVE_DATA_INGESTIONS", "ACTIVE_SUBSCRIPTIONS");
                    Cluster clusterDetails = JsonConvert.DeserializeObject<Cluster>(content)!;
                    Console.WriteLine("Cluster Details");
                    table.AddRow(clusterDetails.Name,
                        clusterDetails.Status.ToString(),
                        clusterDetails.ShardDistributionType.ToString(),
                        clusterDetails.InThroughputInMB,
                        clusterDetails.OutThroughputInMB,
                        clusterDetails.ActiveDataIngestions,
                        clusterDetails.ActiveSubscriptions);

                    table.Write();

                    // shards;
                    Console.WriteLine("Shards connected");
                    int k = 0;
                    foreach (var shard in clusterDetails.Shards)
                    {
                        var shardTable = new ConsoleTable("ID", "TYPE", "REPLICAS");
                        shardTable.AddRow(k, shard.ReplicaDistributionType.ToString(), shard.Replicas.ToJson());
                        shardTable.Write();

                        k++;
                    }
                }
            }
            catch (Exception)
            {
                var table = new ConsoleTable("STATUS", "ERROR");

                table.AddRow("NOT_CONNECTED", $"It can not connect to the node, check network connectivity");
                table.Write();
            }
        }
    }
}
