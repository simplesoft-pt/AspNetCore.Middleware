using System;
using System.Collections.Generic;

namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The health check model returned by the middleware
    /// </summary>
    public class HealthCheckModel
    {
        /// <summary>
        /// The global status
        /// </summary>
        public HealthCheckGlobalStatus Status { get; set; }

        /// <summary>
        /// The date and time when the check started
        /// </summary>
        public DateTimeOffset StartedOn { get; set; }

        /// <summary>
        /// The date and time when the check terminated
        /// </summary>
        public DateTimeOffset TerminatedOn { get; set; }

        /// <summary>
        /// The health check dependencies
        /// </summary>
        public Dictionary<string, HealthCheckDependencyModel> Dependencies { get; set; }
    }
}