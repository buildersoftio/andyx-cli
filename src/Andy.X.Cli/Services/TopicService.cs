using Andy.X.Cli.Utilities;
using Andy.X.Cli.Utilities.Extensions;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using ConsoleTables;
using Newtonsoft.Json;
using System.Text;

namespace Andy.X.Cli.Services
{
    public static class TopicService
    {
        public static void GetTopics(string tenant, string product, string component)
        {
            Console.WriteLine("test");
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "TOPIC");
                    List<string> list = JsonConvert.DeserializeObject<List<string>>(content)!;

                    foreach (var item in list)
                    {
                        table.AddRow(tenant, product, component, item);
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
        public static void GetTopic(string tenant, string product, string component, string topic)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("ID", "TOPIC_NAME", "DESCRIPTION", "UPDATED_DATE", "CREATED_DATE", "UPDATED_BY", "CREATED_BY");

                    var topicDetails = JsonConvert.DeserializeObject<Topic>(content);
                    table.AddRow(topicDetails!.Id, topicDetails.Name, topicDetails.Description, topicDetails.UpdatedDate, topicDetails.CreatedDate, topicDetails.UpdatedBy, topicDetails.CreatedBy);
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
        public static void GetTopicSettings(string tenant, string product, string component, string topic)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/settings";
            Console.WriteLine(request);
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                HttpResponseMessage httpResponseMessage = client.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var table = new ConsoleTable("TENANT", "PRODUCT", "COMPONENT", "TOPIC", "KEY", "VALUE");
                    var topicSettings = JsonConvert.DeserializeObject<TopicSettings>(content);

                    table.AddRow(tenant, product, component, topic, "WriteBufferSizeInBytes", topicSettings!.WriteBufferSizeInBytes);
                    table.AddRow(tenant, product, component, topic, "MaxWriteBufferNumber", topicSettings.MaxWriteBufferNumber);
                    table.AddRow(tenant, product, component, topic, "MaxWriteBufferSizeToMaintain", topicSettings.MaxWriteBufferSizeToMaintain);
                    table.AddRow(tenant, product, component, topic, "MinWriteBufferNumberToMerge", topicSettings.MinWriteBufferNumberToMerge);
                    table.AddRow(tenant, product, component, topic, "MaxBackgroundCompactionsThreads", topicSettings.MaxBackgroundCompactionsThreads);
                    table.AddRow(tenant, product, component, topic, "MaxBackgroundFlushesThreads", topicSettings.MaxBackgroundFlushesThreads);

                    table.Write();

                    // settings description
                    Console.WriteLine("Settings Description "); Console.WriteLine("");
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("1. WriteBufferSizeInBytes ");
                    Console.ForegroundColor = color;
                    Console.WriteLine("write_buffer_size sets the size of a single memtable. Once memtable exceeds this size, it is marked immutable and a new one is created, for now we are creating as 64MB SIZE"); Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("2. MaxWriteBufferNumber ");
                    Console.ForegroundColor = color;
                    Console.WriteLine("max_write_buffer_number sets the maximum number of memtables, both active and immutable. If the active memtable fills up and the total number of memtables is larger than max_write_buffer_number we stall further writes. This may happen if the flush process is slower than the write rate."); Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("3. MaxWriteBufferSizeToMaintain ");
                    Console.ForegroundColor = color;
                    Console.WriteLine("the amount of write history to maintain in memory, in bytes. This includes the current memtable size, sealed but unflushed memtables, and flushed memtables that are kept around. Andy X will try to keep at least this much history in memory - if dropping a flushed memtable would result in history falling below this threshold, it would not be dropped. (Default: 0)"); Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("4. MinWriteBufferNumberToMerge ");
                    Console.ForegroundColor = color;
                    Console.WriteLine("min_write_buffer_number_to_merge is the minimum number of memtables to be merged before flushing to storage. For example, if this option is set to 2, immutable memtables are only flushed when there are two of them - a single immutable memtable will never be flushed. If multiple memtables are merged together, less data may be written to storage since two updates are merged to a single key. However, every Get() must traverse all immutable memtables linearly to check if the key is there. Setting this option too high may hurt read performance."); Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("5. MaxBackgroundCompactionsThreads ");
                    Console.ForegroundColor = color;
                    Console.WriteLine("max_background_compactions is the maximum number of concurrent background compactions. The default is 1, but to fully utilize your CPU and storage you might want to increase this to the minimum of (the number of cores in the system, the disk throughput divided by the average throughput of one compaction thread)."); Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("6. MaxBackgroundFlushesThreads ");
                    Console.ForegroundColor = color;
                    Console.WriteLine("max_background_flushes is the maximum number of concurrent flush operations. It is usually good enough to set this to 1."); Console.WriteLine("");

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
        public static void PostTopic(string tenant, string product, string component, string topic, TopicSettings topicSettings)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                var settings = JsonConvert.SerializeObject(topicSettings);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PostAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Topic '{topic}' at '{tenant}/{product}/{component}' has been created succesfully!");
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
        public static void PutTopicSettings(string tenant, string product, string component, string topic, TopicSettings topicSettings)
        {
            var node = NodeService.GetNode();

            string request = $"{node.NodeUrl}api/v3/tenants/{tenant}/products/{product}/components/{component}/topics/{topic}/settings";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.ApplicationName);
                client.AddBasicAuthorizationHeader(node);

                var settings = JsonConvert.SerializeObject(topicSettings);
                var bodyRequest = new StringContent(settings, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = client.PutAsync(request, bodyRequest).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Settings have been updated, '{topic}' is marked to refresh settings, this may take a while!");
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
