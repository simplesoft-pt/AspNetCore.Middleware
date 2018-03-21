namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check middleware options
    /// </summary>
    public class HealthCheckOptions
    {
        /// <summary>
        /// Path for which the middleware responds
        /// </summary>
        public string Path { get; set; } = "api/_health";
    }
}