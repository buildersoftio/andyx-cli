namespace Buildersoft.Andy.X.Model.Entities.Core.Components
{
    public class ComponentSettings
    {
        public bool IsTopicAutomaticCreationAllowed { get; set; }
        public bool IsSchemaValidationEnabled { get; set; }
        public bool IsAuthorizationEnabled { get; set; }

        public bool IsSubscriptionAutomaticCreationAllowed { get; set; }
        public bool IsProducerAutomaticCreationAllowed { get; set; }
    }
}
