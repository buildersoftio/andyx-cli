using Andy.X.Cli.Models.Configurations;
using Andy.X.Cli.Services;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Producers;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Subscriptions;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Cocona;
using ConsoleTables;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

var builder = CoconaApp.CreateBuilder(configureOptions: options =>
{
    options.EnableShellCompletionSupport = true;
});

var app = builder.Build();

app.AddSubCommand("node", x =>
{
    x.AddCommand("connect", (
        [Option(Description = "Url of Andy X Node, default value is 'https://localhost:6541'")] string url,
        [Option('u', Description = "Username of Andy X Node, default is 'admin'")] string? username,
        [Option('p', Description = "Password of Andy X Node, default is 'admin'")] string? password) =>
    {

        username ??= "admin";
        password ??= "admin";

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

    x.AddCommand("show", () =>
    {
        var table = new ConsoleTable("ID", "NODE_URL", "USERNAME", "PASSWORD");
        var node = NodeService.GetNode();
        table.AddRow(1, node.NodeUrl, node.Username, "**********");
        table.Write();

    }).WithDescription("Read node details");
}).WithDescription("Connect and read node details");

app.AddCommand("tenant", ([Argument()] string? tenant,
    [Option(Description = "If his property is set, you will interact with tenant settings")] bool? settings,
    [Option(Description = "Allow product automatic creation from clients, default is 'true'")] bool? allowProductCreation,
    [Option(Description = "Enable encryption of data at rest and in motion, default is 'false'")] bool? enableEncryption,
    [Option(Description = "Enable Authorization, default is 'false'")] bool? enableAuthorization,
    [Option(Description = "Create or read Tenant, unset is read, set is create")] bool? create,
    [Option(Description = "If this property is set it will update the tenant settings, make sure to update all settings you want to update")] bool? update) =>
{
    if (tenant == null && settings.HasValue != true && create.HasValue != true && update.HasValue != true)
    {
        TenantService.GetTenants();
        return;
    }

    if (tenant != null && create.HasValue != true && settings.HasValue != true && update.HasValue != true)
    {
        TenantService.GetTenant(tenant);
        return;
    }

    if (tenant != null && create.HasValue != true && settings == true && update.HasValue != true)
    {
        TenantService.GetTenantSettings(tenant);
        return;
    }

    // create or update
    if (allowProductCreation.HasValue != true)
        allowProductCreation = true;
    if (enableEncryption.HasValue != true)
        enableEncryption = false;
    if (enableAuthorization.HasValue != true)
        enableAuthorization = false;

    if (create == true && update.HasValue != true)
    {
        TenantService.PostTenant(tenant!, new TenantSettings()
        {
            IsProductAutomaticCreationAllowed = allowProductCreation.Value,
            IsEncryptionEnabled = enableEncryption.Value,
            IsAuthorizationEnabled = enableAuthorization.Value,
        });
        return;
    }

    if (create.HasValue != true && update == true)
    {
        TenantService.PutTenantSettings(tenant!, new TenantSettings()
        {
            IsProductAutomaticCreationAllowed = allowProductCreation.Value,
            IsEncryptionEnabled = enableEncryption.Value,
            IsAuthorizationEnabled = enableAuthorization.Value,
        });
        return;
    }



}).WithDescription("Create and read tenants").WithAliases("t");

app.AddCommand("product", ([Argument()] string? product, string tenant,
    [Option(Description = "If his property is set, you will interact with product settings")] bool? settings,
    [Option(Description = "Enable Authorization, default is 'false'")] bool? enableAuthorization,
    [Option(Description = "Create or read Product, unset is read, set is create")] bool? create,
    [Option(Description = "If this property is set it will update the product settings, make sure to update all settings you want to update")] bool? update) =>
{
    if (product == null && settings.HasValue != true && create.HasValue != true && update.HasValue != true)
    {
        ProductService.GetProducts(tenant);
        return;
    }

    if (product != null && create.HasValue != true && settings.HasValue != true && update.HasValue != true)
    {
        ProductService.GetProduct(tenant, product);
        return;
    }

    if (product != null && create.HasValue != true && settings == true && update.HasValue != true)
    {
        ProductService.GetProductSettings(tenant, product);
        return;
    }

    // create or update
    if (enableAuthorization.HasValue != true)
        enableAuthorization = false;

    if (product != null && create == true && update.HasValue != true)
    {
        ProductService.PostProduct(tenant, product!, new ProductSettings()
        {
            IsAuthorizationEnabled = enableAuthorization!.Value,
        });
    }

    if (product != null && create.HasValue != true && update == true)
    {
        ProductService.PutProductSettings(tenant, product, new ProductSettings()
        {
            IsAuthorizationEnabled = enableAuthorization!.Value,
        });
    }

}).WithDescription("Create and read products").WithAliases("p");

app.AddCommand("component", ([Argument()] string? component, string tenant, string product,
    [Option(Description = "If his property is set, you will interact with product settings")] bool? settings,
    [Option(Description = "Allow topic automatic creation from clients, default is 'true'")] bool? enableTopicCreation,
    [Option(Description = "Allow subscription automatic creation from clients, default is 'true'")] bool? enableSubscriptionCreation,
    [Option(Description = "Allow producers automatic creation from clients, default is 'true'")] bool? enableProducerCreation,
    [Option(Description = "Enable Authorization, default is 'false'")] bool? enableAuthorization,
    [Option(Description = "Enforce Schema Validation for topics created in this component, default is 'false'")] bool? enforceSchemaValidation,
    [Option(Description = "Create or read Component, unset is read, set is create")] bool? create,
    [Option(Description = "If this property is set it will update the product settings, make sure to update all settings you want to update")] bool? update) =>
{
    if (component == null && settings.HasValue != true && create.HasValue != true && update.HasValue != true)
    {
        ComponentService.GetComponents(tenant, product);
        return;
    }

    if (component != null && create.HasValue != true && settings.HasValue != true && update.HasValue != true)
    {
        ComponentService.GetComponent(tenant, product, component);
        return;
    }

    if (component != null && create.HasValue != true && settings == true && update.HasValue != true)
    {
        ComponentService.GetComponentSettings(tenant, product, component);
        return;
    }

    // create or update
    if (enableAuthorization.HasValue != true)
        enableAuthorization = false;

    if (enableTopicCreation.HasValue != true)
        enableTopicCreation = true;

    if (enableSubscriptionCreation.HasValue != true)
        enableSubscriptionCreation = true;

    if (enableProducerCreation.HasValue != true)
        enableProducerCreation = true;

    if (enforceSchemaValidation.HasValue != true)
        enforceSchemaValidation = false;



    if (component != null && create == true && update.HasValue != true)
    {
        ComponentService.PostComponent(tenant, product!, component!, new ComponentSettings()
        {
            IsAuthorizationEnabled = enableAuthorization!.Value,
            IsTopicAutomaticCreationAllowed = enableTopicCreation.Value,
            IsSubscriptionAutomaticCreationAllowed = enableSubscriptionCreation.Value,
            EnforceSchemaValidation = enforceSchemaValidation.Value,
            IsProducerAutomaticCreationAllowed = enableProducerCreation.Value,
        });
    }

    if (component != null && create.HasValue != true && update == true)
    {
        ComponentService.PutComponentSettings(tenant, product!, component!, new ComponentSettings()
        {
            IsAuthorizationEnabled = enableAuthorization!.Value,
            IsTopicAutomaticCreationAllowed = enableTopicCreation.Value,
            IsSubscriptionAutomaticCreationAllowed = enableSubscriptionCreation.Value,
            EnforceSchemaValidation = enforceSchemaValidation.Value
        });
    }

}).WithDescription("Create and read components");

app.AddCommand("topic", ([Argument()] string? topic, string tenant, string product, string component,
    [Option(Description = "If his property is set, you will interact with product settings")] bool? settings,
    [Option(Description = "Topic Settings, if this property is not set, the default values will be applied for this topic")] TopicSettings? topicSettings,
    [Option(Description = "Create or read Topic, unset is read, set is create")] bool? create,
    [Option(Description = "If this property is set it will update the product settings, make sure to update all settings you want to update")] bool? update) =>
{
    if (topic == null && settings.HasValue != true && create.HasValue != true && update.HasValue != true)
    {
        TopicService.GetTopics(tenant, product, component);
        return;
    }

    if (topic != null && create.HasValue != true && settings.HasValue != true && update.HasValue != true)
    {
        TopicService.GetTopic(tenant, product, component, topic);
        return;
    }

    if (topic != null && create.HasValue != true && settings == true && update.HasValue != true)
    {
        TopicService.GetTopicSettings(tenant, product, component, topic);
        return;
    }

    // create or update
    topicSettings ??= new TopicSettings()
    {
        WriteBufferSizeInBytes = 64000000,
        MaxWriteBufferNumber = 4,
        MaxWriteBufferSizeToMaintain = 0,
        MinWriteBufferNumberToMerge = 2,
        MaxBackgroundCompactionsThreads = 1,
        MaxBackgroundFlushesThreads = 1,
    };

    if (topic != null && create == true && update.HasValue != true)
    {
        TopicService.PostTopic(tenant, product, component, topic!, topicSettings);
    }

    if (topic != null && create.HasValue != true && update == true)
    {
        TopicService.PutTopicSettings(tenant, product, component, topic!, topicSettings);
    }

}).WithDescription("Create and read topics");

app.AddCommand("subscription", ([Argument()] string? subscription, string tenant, string product, string component, string topic,
    [Option(Description = "Subscription type, the default is 'Unique'")] SubscriptionType? type,
    [Option(Description = "Subscription mode, the default value is 'NonResilient'")] SubscriptionMode? mode,
    [Option(Description = "Initial Position for subscription, the default value is 'Latest'")] InitialPosition? initialPosition,
    [Option(Description = "Create or read Subscription, unset is read, set is create")] bool? create) =>
{
    if (subscription == null && create.HasValue != true)
    {
        SubscriptionService.GetSubscriptions(tenant, product, component, topic);
        return;
    }

    if (subscription != null && create.HasValue != true)
    {
        SubscriptionService.GetSubscription(tenant, product, component, topic, subscription);
        return;
    }

    type ??= SubscriptionType.Unique;
    mode ??= SubscriptionMode.NonResilient;
    initialPosition ??= InitialPosition.Latest;

    if (topic != null && create == true)
    {
        SubscriptionService.PostSubscription(tenant, product, component, topic, subscription!, type.Value, mode.Value, initialPosition.Value);
    }

}).WithDescription("Create and read subscriptions");

app.AddCommand("producer", ([Argument()] string? producer, string tenant, string product, string component, string topic,
    [Option(Description = "Producer Description")] string? description,
    [Option(Description = "Instance type, the default is 'Multiple'")] ProducerInstanceType? type,
    [Option(Description = "Create or read Subscription, unset is read, set is create")] bool? create) =>
{
    if (producer == null && create.HasValue != true)
    {
        ProducerService.GetProducers(tenant, product, component, topic);
        return;
    }

    if (producer != null && create.HasValue != true)
    {
        ProducerService.GetProducer(tenant, product, component, topic, producer);
        return;
    }

    type ??= ProducerInstanceType.Multiple;
    description ??= "created from andyx cli";


    if (topic != null && create == true)
    {
        ProducerService.PostProducer(tenant, product, component, topic, producer!, description!, type.Value);
    }

}).WithDescription("Create and read producers");


app.AddSubCommand("authorize", x =>
{
    x.AddCommand("tenant", (
        string tenant,
        [Argument()] Guid? key,
        [Option(Description = "Token description")] string? description,
        [Option(Description = "Roles, by default two roles will be asign Consume and Produce")] List<TenantTokenRole>? roles,
        [Option(Description = "Expire date, date when the secret is valid, if is not set the default value is 2 years from this moment.")] DateTime? expireDate,
        [Option(Description = "Create or read Tenant Tokens, unset is read, set is create")] bool? create,
        [Option(Description = "Revoke the given key, key should have value to be able to revoke a key")] bool? revoke) =>
    {

        if (key.HasValue != true && create.HasValue != true && revoke.HasValue != true)
        {
            TenantTokenService.GetTenantTokens(tenant);
            return;
        }

        if (key.HasValue == true && create.HasValue != true && revoke.HasValue != true)
        {
            TenantTokenService.GetTenantToken(tenant, key.Value);
            return;
        };

        if (key.HasValue != true && create.HasValue == true && revoke.HasValue != true)
        {
            if (expireDate.HasValue != true)
            {
                expireDate = DateTime.UtcNow.AddYears(2);
            }

            description ??= "created by andyx cli";
            roles ??= new List<TenantTokenRole>() { TenantTokenRole.Produce, TenantTokenRole.Consume };

            var tenantToken = new TenantToken()
            {
                Description = description,
                IsActive = true,
                Roles = roles,
                ExpireDate = expireDate.Value,
                IssuedDate = DateTime.Now
            };

            TenantTokenService.PostTenantToken(tenant, tenantToken);
            return;
        };

        if (key.HasValue == true && create.HasValue != true && revoke.HasValue == true)
        {
            TenantTokenService.RevokeTenantToken(tenant, key.Value);
            return;
        };

        Console.WriteLine("Wrong combination");


    }).WithDescription("Manage tenant keys");

    x.AddCommand("product", (
        string tenant, string product,
        [Argument()] Guid? key,
        [Option(Description = "Token description")] string? description,
        [Option(Description = "Roles")] List<ProductTokenRole>? roles,
        [Option(Description = "Expire date, date when the secret is valid, if is not set the default value is 2 years from this moment.")] DateTime? expireDate,
        [Option(Description = "Create or read Product Tokens, unset is read, set is create")] bool? create,
        [Option(Description = "Revoke the given key, key should have value to be able to revoke a key")] bool? revoke) =>
    {
        if (key.HasValue != true && create.HasValue != true && revoke.HasValue != true)
        {
            ProductTokenService.GetProductTokens(tenant, product);
            return;
        }

        if (key.HasValue == true && create.HasValue != true && revoke.HasValue != true)
        {
            ProductTokenService.GetProductToken(tenant, product, key.Value);
            return;
        };

        if (key.HasValue != true && create.HasValue == true && revoke.HasValue != true)
        {
            if (expireDate.HasValue != true)
            {
                expireDate = DateTime.UtcNow.AddYears(2);
            }
            description ??= "created by andyx cli";
            roles ??= new List<ProductTokenRole>() { ProductTokenRole.Produce, ProductTokenRole.Consume };

            var productToken = new ProductToken()
            {
                Description = description,
                IsActive = true,
                Roles = roles,
                ExpireDate = expireDate.Value,
                IssuedDate = DateTime.Now
            };

            ProductTokenService.PostProductToken(tenant, product, productToken);
            return;
        };

        if (key.HasValue == true && create.HasValue != true && revoke.HasValue == true)
        {
            ProductTokenService.RevokeProductToken(tenant, product, key.Value);
            return;
        };

        Console.WriteLine("Wrong combination");


    }).WithDescription("Manage product keys");

    x.AddCommand("component", (
        string tenant, string product, string component,
        [Argument()] Guid? key,
        [Option(Description = "Enter the name of the client you would like to grant access (Producer or Subscription)")] string? issuedFor,
        [Option(Description = "Token description")] string? description,
        [Option(Description = "Roles")] List<ComponentTokenRole>? roles,
        [Option(Description = "Expire date, date when the secret is valid, if is not set the default value is 2 years from this moment.")] DateTime? expireDate,
        [Option(Description = "Create or read Component Tokens, unset is read, set is create")] bool? create,
        [Option(Description = "Revoke the given key, key should have value to be able to revoke a key")] bool? revoke) =>
    {
        if (key.HasValue != true && create.HasValue != true && revoke.HasValue != true)
        {
            ComponentTokenService.GetComponentTokens(tenant, product, component);
            return;
        }

        if (key.HasValue == true && create.HasValue != true && revoke.HasValue != true)
        {
            ComponentTokenService.GetComponentToken(tenant, product, component, key.Value);
            return;
        };

        if (key.HasValue != true && create.HasValue == true && revoke.HasValue != true)
        {
            if (expireDate.HasValue != true)
            {
                expireDate = DateTime.UtcNow.AddYears(2);
            }
            description ??= "created by andyx cli";
            roles ??= new List<ComponentTokenRole>() { ComponentTokenRole.Produce, ComponentTokenRole.Consume };

            issuedFor ??= "";

            var componentToken = new ComponentToken()
            {
                Description = description,
                IsActive = true,
                IssuedFor = issuedFor,
                Roles = roles,
                ExpireDate = expireDate.Value,
                IssuedDate = DateTime.Now
            };

            ComponentTokenService.PostComponentToken(tenant, product, component, componentToken);
            return;
        };

        if (key.HasValue == true && create.HasValue != true && revoke.HasValue == true)
        {
            ComponentTokenService.RevokeComponentToken(tenant, product, component, key.Value);
            return;
        };

        Console.WriteLine("Wrong combination");

    }).WithDescription("Manage component keys");


}).WithDescription("Manage api keys and secrets for tenants, products and components");

app.AddSubCommand("retention", x =>
{
    x.AddCommand("tenant", (
        string tenant,
        [Argument()] long? id,
        [Option(Description = "Retention Policy Name, default is 'default_ttl'")] string? name,
        [Option(Description = "Retention Policy Type, default is SOFT_TTL. In case of update type can not change")] RetentionType? type,
        [Option(Description = "Time to live in minutes, default value is 43800 (one month)")] long? ttl,
        [Option(Description = "Create a retention policy for tenant, unset is read, set is create")] bool? create,
        [Option(Description = "update a retention policy for tenant with given id, set is update")] bool? update,
        [Option(Description = "delete a retention policy for tenant with given id, set is delete")] bool? delete
        ) =>
    {
        if (id.HasValue != true && create.HasValue != true && update.HasValue != true && delete.HasValue != true)
        {
            TenantRetentionService.GetTenantRetentions(tenant);
            return;
        }

        if (id.HasValue != true && create.HasValue == true && update.HasValue != true && delete.HasValue != true)
        {
            if (name == null)
                name = "default_ttl";
            if (type.HasValue != true)
                type = RetentionType.SOFT_TTL;
            if (ttl.HasValue != true)
                ttl = 43800;

            var retention = new TenantRetention()
            {
                Name = name,
                TimeToLiveInMinutes = ttl.Value,
                Type = type.Value
            };

            TenantRetentionService.PostTenantRetention(tenant, retention);
            return;
        }

        if (id.HasValue == true && create.HasValue != true && update.HasValue == true && delete.HasValue != true)
        {
            if (name == null)
                name = "default_ttl";
            if (type.HasValue != true)
                type = RetentionType.SOFT_TTL;
            if (ttl.HasValue != true)
                ttl = 43800;

            var retention = new TenantRetention()
            {
                Name = name,
                TimeToLiveInMinutes = ttl.Value,
                Type = type.Value
            };

            TenantRetentionService.UpdateTenantRetention(tenant, id.Value, retention);
            return;
        }

        if (id.HasValue == true && create.HasValue != true && update.HasValue != true && delete.HasValue == true)
        {
            TenantRetentionService.DeleteTenantRetention(tenant, id.Value);
            return;
        }

        Console.WriteLine("Wrong combination");

    }).WithDescription("Manage retention policies for tenant");

    x.AddCommand("product", (
        string tenant, string product,
        [Argument()] long? id,
        [Option(Description = "Retention Policy Name, default is 'default_ttl'")] string? name,
        [Option(Description = "Retention Policy Type, default is SOFT_TTL. In case of update type can not change")] RetentionType? type,
        [Option(Description = "Time to live in minutes, default value is 43800 (one month)")] long? ttl,
        [Option(Description = "Create a retention policy for tenant, unset is read, set is create")] bool? create,
        [Option(Description = "update a retention policy for tenant with given id, set is update")] bool? update,
        [Option(Description = "delete a retention policy for tenant with given id, set is delete")] bool? delete
        ) =>
    {
        if (id.HasValue != true && create.HasValue != true && update.HasValue != true && delete.HasValue != true)
        {
            ProductRetentionService.GetProductRetentions(tenant, product);
            return;
        }

        if (id.HasValue != true && create.HasValue == true && update.HasValue != true && delete.HasValue != true)
        {
            if (name == null)
                name = "default_ttl";
            if (type.HasValue != true)
                type = RetentionType.SOFT_TTL;
            if (ttl.HasValue != true)
                ttl = 43800;

            var retention = new ProductRetention()
            {
                Name = name,
                TimeToLiveInMinutes = ttl.Value,
                Type = type.Value
            };

            ProductRetentionService.PostProductRetention(tenant, product, retention);
            return;
        }

        if (id.HasValue == true && create.HasValue != true && update.HasValue == true && delete.HasValue != true)
        {
            if (name == null)
                name = "default_ttl";
            if (type.HasValue != true)
                type = RetentionType.SOFT_TTL;
            if (ttl.HasValue != true)
                ttl = 43800;

            var retention = new ProductRetention()
            {
                Name = name,
                TimeToLiveInMinutes = ttl.Value,
                Type = type.Value
            };

            ProductRetentionService.UpdateProductRetention(tenant, product, id.Value, retention);
            return;
        }

        if (id.HasValue == true && create.HasValue != true && update.HasValue != true && delete.HasValue == true)
        {
            ProductRetentionService.DeleteProductRetention(tenant, product, id.Value);
            return;
        }

        Console.WriteLine("Wrong combination");

    }).WithDescription("Manage retention policies for product");

    x.AddCommand("component", (
        string tenant, string product, string component,
        [Argument()] long? id,
        [Option(Description = "Retention Policy Name, default is 'default_ttl'")] string? name,
        [Option(Description = "Retention Policy Type, default is SOFT_TTL. In case of update type can not change")] RetentionType? type,
        [Option(Description = "Time to live in minutes, default value is 43800 (one month)")] long? ttl,
        [Option(Description = "Create a retention policy for tenant, unset is read, set is create")] bool? create,
        [Option(Description = "update a retention policy for tenant with given id, set is update")] bool? update,
        [Option(Description = "delete a retention policy for tenant with given id, set is delete")] bool? delete
        ) =>
    {
        if (id.HasValue != true && create.HasValue != true && update.HasValue != true && delete.HasValue != true)
        {
            ComponentRetentionService.GetComponentRetentions(tenant, product, component);
            return;
        }

        if (id.HasValue != true && create.HasValue == true && update.HasValue != true && delete.HasValue != true)
        {
            if (name == null)
                name = "default_ttl";
            if (type.HasValue != true)
                type = RetentionType.SOFT_TTL;
            if (ttl.HasValue != true)
                ttl = 43800;

            var retention = new ComponentRetention()
            {
                Name = name,
                TimeToLiveInMinutes = ttl.Value,
                Type = type.Value
            };

            ComponentRetentionService.PostComponentRetention(tenant, product, component, retention);
            return;
        }

        if (id.HasValue == true && create.HasValue != true && update.HasValue == true && delete.HasValue != true)
        {
            if (name == null)
                name = "default_ttl";
            if (type.HasValue != true)
                type = RetentionType.SOFT_TTL;
            if (ttl.HasValue != true)
                ttl = 43800;

            var retention = new ComponentRetention()
            {
                Name = name,
                TimeToLiveInMinutes = ttl.Value,
                Type = type.Value
            };

            ComponentRetentionService.UpdateComponentRetention(tenant, product, component, id.Value, retention);
            return;
        }

        if (id.HasValue == true && create.HasValue != true && update.HasValue != true && delete.HasValue == true)
        {
            ComponentRetentionService.DeleteComponentRetention(tenant, product, component, id.Value);
            return;
        }

        Console.WriteLine("Wrong combination");

    }).WithDescription("Manage retention policies for component");


}).WithDescription("Manage life time of data at tenants, products and components");

app.Run();