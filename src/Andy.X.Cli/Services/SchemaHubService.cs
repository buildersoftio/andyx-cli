using Andy.X.Cli.IO.Locations;
using Andy.X.Cli.Models.Configurations;
using Andy.X.Cli.Utilities.Extensions;

namespace Andy.X.Cli.Services
{
    public static class SchemaHubService
    {
        public static bool AddSchemaHub(string nodeUrl, string username, string password)
        {
            if (nodeUrl.EndsWith("/") != true)
                nodeUrl = nodeUrl + "/";

            var node = new Node() { NodeUrl = nodeUrl, Username = username, Password = password };
            if (File.Exists(ConfigurationLocations.GetSchemaConfigurationFile()))
                File.Delete(ConfigurationLocations.GetSchemaConfigurationFile());

            try
            {
                File.WriteAllText(ConfigurationLocations.GetSchemaConfigurationFile(), node.ToPrettyJson());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Node GetSchemaHub()
        {
            return File.ReadAllText(ConfigurationLocations.GetSchemaConfigurationFile()).JsonToObject<Node>();
        }
    }
}
