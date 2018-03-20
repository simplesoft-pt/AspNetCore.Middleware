namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The health check status
    /// </summary>
    public enum HealthCheckStatus
    {
        /// <summary>
        /// Green status - health check succeded
        /// </summary>
        Green,
        /// <summary>
        /// Red status - health check failed
        /// </summary>
        Red
    }
}