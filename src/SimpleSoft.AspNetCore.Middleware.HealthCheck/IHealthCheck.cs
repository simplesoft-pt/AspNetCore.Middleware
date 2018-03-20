using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// The health check name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The health check status
        /// </summary>
        HealthCheckStatus Status { get; }

        /// <summary>
        /// Is the health check required?
        /// </summary>
        bool Required { get; }

        /// <summary>
        /// Collection of tags/categories
        /// </summary>
        IReadOnlyCollection<string> Tags { get; }

        /// <summary>
        /// Updates the health check status
        /// </summary>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited</returns>
        Task UpdateStatusAsync(CancellationToken ct);
    }
}
