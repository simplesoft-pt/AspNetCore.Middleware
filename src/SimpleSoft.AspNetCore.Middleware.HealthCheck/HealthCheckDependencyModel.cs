namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The health check dependency model returned by the middleware
    /// </summary>
    public class HealthCheckDependencyModel
    {
        /// <summary>
        /// The current status
        /// </summary>
        public HealthCheckStatus Status { get; set; }

        /// <summary>
        /// Is the dependency required?
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// A collection of tags
        /// </summary>
        public string[] Tags { get; set; }
    }
}