namespace Buildersoft.Andy.X.Model.Entities.Core.Subscriptions
{
    public class Subscription
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public InitialPosition InitialPosition { get; set; }

        public List<string> PublicIpRange { get; set; }
        public List<string> PrivateIpRange { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }

    public enum SubscriptionType
    {
        /// <summary>
        /// Only one consumer
        /// </summary>
        Unique,
        /// <summary>
        /// One consumer with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one consumer.
        /// </summary>
        Shared
    }

    public enum InitialPosition
    {
        Earliest,
        Latest
    }

    public enum SubscriptionMode
    {
        /// <summary>
        /// Durable
        /// </summary>
        Resilient,

        /// <summary>
        /// Non Durable
        /// </summary>
        NonResilient
    }
}
