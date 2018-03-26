namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check middleware options
    /// </summary>
    public class HealthCheckOptions : SimpleSoftMiddlewareOptions
    {
        /// <summary>
        /// Path for which the middleware responds. Defaults to 'api/_health'.
        /// </summary>
        public string Path { get; set; } = "api/_health";

        /// <summary>
        /// Indent the JSON response? Defaults to 'true'.
        /// </summary>
        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// Should enums be returned as strings? Defaults to 'true'.
        /// </summary>
        public bool StringEnum { get; set; } = true;
    }
}