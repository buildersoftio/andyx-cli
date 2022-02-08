using Andy.X.Cli.Models.Configurations;
using Andy.X.Cli.Services;
using Cocona;
using ConsoleTables;

var builder = CoconaApp.CreateBuilder();
var app = builder.Build();

app.AddSubCommand("node", x =>
{
    x.AddCommand("connect", (
        [Option(Description = "Url of Andy X Node, default value is 'https://localhost:6541'")] string url,
        [Option('u', Description = "Username of Andy X Node, default is admin")] string? username,
        [Option('p', Description = "Password of Andy X Node, default is admin")] string? password) =>
    {

        if (username == null)
            username = "";
        if (password == null)
            password = "";
        var isConnected = NodeService.AddNode(url, username, password);
        if (isConnected)
        {
            Console.WriteLine();
            Console.WriteLine($"Node '{url}' is registered");

            var table = new ConsoleTable("ID", "NODE_URL", "USERNAME", "PASSWORD");
            var node = NodeService.GetNode();
            table.AddRow(1, node.NodeUrl, node.Username, node.Password);
            table.Write();
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"Something went wrong! Node '{url}' is not registered.");
        }

    }).WithDescription("Connect to a node");

    x.AddCommand("list", () =>
    {
        var table = new ConsoleTable("ID", "NODE_URL", "USERNAME", "PASSWORD");
        var node = NodeService.GetNode();
        table.AddRow(1, node.NodeUrl, node.Username, node.Password);
        table.Write();

    }).WithDescription("Read node details");
});

app.AddCommand("tenant", ([Argument()] string? tenant,
    [Option(Description = "Digital Signature, a unique signature to generate tokes")] string? digitalSignature,
    [Option(Description = "Allow Product Creation, default is true")] bool? allowProductCreation,
    [Option(Description = "Enable Message Encryption in rest and in motion, default is false")] bool? enableEncryption,
    [Option(Description = "Enable GeoReplication, default is false")] bool? enableGeoReplication,
    [Option(Description = "Enable Authorization, default is false")] bool? enableAuthorization,
    [Option(Description = "Certificate Location")] string? certificatePath,
    [Option(Description = "Logging Level, default is ALL")] TenantLogging? logging,
    [Option(Description = "Create or read Tenant, unset is read, set is create")] bool? create) =>
{
    if (tenant == null)
    {
        TenantService.GetTenants();
        return;
    }
    if (tenant != null && create.HasValue != true)
    {
        TenantService.GetTenant(tenant);
        return;
    }

    //create
    if (allowProductCreation.HasValue != true)
        allowProductCreation = true;
    if (enableEncryption.HasValue != true)
        enableEncryption = false;
    if (enableGeoReplication.HasValue != true)
        enableGeoReplication = false;
    if (enableAuthorization.HasValue != true)
        enableAuthorization = false;
    if (digitalSignature == null)
        digitalSignature = "KWDjwhAndyjp370qwetM2DFS43BuilderSoft";
    if (certificatePath == null)
        certificatePath = "";
    if (logging.HasValue != true)
        logging = TenantLogging.ALL;

    TenantService.PostTenant(tenant, new Andy.X.Cli.Models.Configurations.TenantSettings()
    {
        AllowProductCreation = allowProductCreation.Value,
        EnableEncryption = enableEncryption.Value,
        EnableGeoReplication = enableGeoReplication.Value,
        EnableAuthorization = enableAuthorization.Value,
        DigitalSignature = digitalSignature,
        Logging = Andy.X.Cli.Models.Configurations.TenantLogging.ALL,
        CertificatePath = certificatePath,
    });


}).WithDescription("Read and Create Tenants").WithAliases("t");

app.AddSubCommand("authorization", x =>
{
    x.AddCommand("tenant", ([Option(Description = "Tenant name")] string tenant,
        [Option(Description = "Date when token expires, default is 2 years starting from now")] DateTime? expireDate,
        [Option(Description = "Create or read Tenant Token, unset is read, set is create")] bool? create) =>
    {
        if (create.HasValue == true)
        {
            if (expireDate.HasValue != true)
                expireDate = DateTime.Now.AddYears(2);

            TenantService.PostTenantToken(tenant, expireDate.Value);
        }
        else
            TenantService.GetTenantTokens(tenant);
    });
    x.AddCommand("component", ([Option(Description = "Tenant name")] string tenant,
        [Option(Description = "Product name")] string product,
        [Option(Description = "Component name")] string component,
        [Option(Description = "Date when token expires, default is 2 years starting from now")] DateTime? expireDate,
        [Option(Description = "Name of the token, example which app will use")] string? name,
        [Option(Description = "Descriptoon of the token")] string? description,
        [Option(Description = "Can this token works with consumers, default is true")] bool? canConsume,
        [Option(Description = "Can this token works with producers, default is true")] bool? canProduce,
        [Option(Description = "To whom this token is issued")] string? issueFor,
        [Option(Description = "Create or read Component Token, unset is read, set is create")] bool? create) =>
    {
        // check default values first
        if (create.HasValue == true)
        {
            if (expireDate.HasValue != true)
                expireDate = DateTime.Now.AddYears(2);
            if (name == null)
                name = "";
            if (description == null)
                description = "";
            if (canConsume.HasValue != true)
                canConsume = true;
            if (canProduce.HasValue != true)
                canProduce = true;
            if (issueFor == null)
                issueFor = "";

            ComponentService.PostComponentToken(tenant, product, component, new ComponentToken()
            {
                CanConsume = canConsume.Value,
                CanProduce = canProduce.Value,
                ExpireDate = expireDate.Value,
                Description = description,
                Name = name,
                IsActive = true,
                IssuedDate = DateTime.Now,
                IssuedFor = issueFor,
                Token = "",
            });
        }
        else
            ComponentService.GetComponentTokens(tenant, product, component);
    });
}).WithDescription("Read and Create Tenants").WithAliases("auth");

app.AddCommand("product", (string tenant, [Argument] string? product, bool? create) =>
{
    if (product == null)
    {
        ProductService.GetProducts(tenant);
        return;
    }
    if (create == true)
    {
        Console.WriteLine("Create product not implemented");
        return;
    }

    ProductService.GetProduct(tenant, product);

}).WithDescription("Read and Create Products").WithAliases("p");

app.AddCommand("component", (string tenant, string product, [Argument] string? component, bool? create) =>
{
    if (component == null)
    {
        ComponentService.GetComponents(tenant, product);
        return;
    }
    if (create == true)
    {
        Console.WriteLine("Create product not implemented");
        return;
    }

    ComponentService.GetComponent(tenant, product, component);
}).WithDescription("Read and Create Components").WithAliases("c");

app.AddCommand("topic", (string tenant, string product, string component, [Argument] string? topic, bool? create) =>
{
    if (topic == null)
    {
        TopicService.GetTopics(tenant, product, component);
        return;
    }
    if (create == true)
    {
        Console.WriteLine("Create product not implemented");
        return;
    }

    TopicService.GetTopic(tenant, product, component, topic);
}).WithDescription("Read and Create Topics");


app.AddCommand("consumer", ([Argument()] string? consumer) =>
{
    if (consumer == null)
        ConsumerService.GetConsumers();
    else
        ConsumerService.GetConsumer(consumer);

}).WithDescription("Read consumers detail");

app.AddCommand("producer", ([Argument()] string? producer) =>
{
    if (producer == null)
        ProducerService.GetProducers();
    else
        ProducerService.GetProducer(producer);
}).WithDescription("Read producers detail");


app.AddCommand("build", ([Argument] string app) =>
{
    Console.WriteLine("not implemented");
}).WithDescription("Build Andy X Extensions and Plugins");

app.AddCommand("restore", ([Argument] string app) =>
{
    Console.WriteLine("not implemented");
}).WithDescription("Restore Andy X Extensions and Plugins");


app.AddCommand("pack", ([Argument] string app, string outputLocation) =>
{
    Console.WriteLine("not implemented");
}).WithDescription("Pack Andy X Extensions and Plugins");

app.AddCommand("push", ([Argument] string app, [Argument] string outputLocation, string? apiKey) =>
{
    Console.WriteLine(app);
    Console.WriteLine("Push");
}).WithDescription("Push Andy X Extensions and Plugins to Andy X Hub");

app.Run();