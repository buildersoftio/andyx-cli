namespace Andy.X.Cli.IO.Locations
{
    public static class ConfigurationLocations
    {
        public static string GetRootDirectory()
        {
            // this location should be configured from the client.
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string ConfigDirectory()
        {
            return Path.Combine(GetRootDirectory(), "config");
        }

        public static string GetNodeConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "node.json");
        }

        public static string GetTenantsConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "tenants.json");
        }

        public static string GetUsersConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "users_config.json");
        }

        public static string GetClustersConfigurationFile()
        {
            return Path.Combine(ConfigDirectory(), "clusters_config.json");
        }
    }

}
