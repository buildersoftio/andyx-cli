using Andy.X.Cli.IO.Locations;
using Andy.X.Cli.Models.Configurations;
using Andy.X.Cli.Utilities.Extensions;

namespace Andy.X.Cli.Services
{
    public static class NodeService
    {
        public static bool AddNode(string nodeUrl, string username, string password)
        {
            if (nodeUrl.EndsWith("/") != true)
                nodeUrl = nodeUrl + "/";

            var node = new Node() { NodeUrl = nodeUrl, Username = username, Password = password };
            if (File.Exists(ConfigurationLocations.GetNodeConfigurationFile()))
                File.Delete(ConfigurationLocations.GetNodeConfigurationFile());

            try
            {
                File.WriteAllText(ConfigurationLocations.GetNodeConfigurationFile(), node.ToPrettyJson());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Node GetNode()
        {
            return File.ReadAllText(ConfigurationLocations.GetNodeConfigurationFile()).JsonToObject<Node>();
        }
    }
}
