namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The global health check status
    /// </summary>
    public enum HealthCheckGlobalStatus
    {
        /// <summary>
        /// Green status - no problems have been detected
        /// </summary>
        Green,
        /// <summary>
        /// Yellow status - at least one non required health check failed
        /// </summary>
        Yellow,
        /// <summary>
        /// Red status - at least one required health check failed
        /// </summary>
        Red
    }
}