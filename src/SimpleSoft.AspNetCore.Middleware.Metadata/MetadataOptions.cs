using System;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Metadata middleware options
    /// </summary>
    public class MetadataOptions
    {
        /// <summary>
        /// Path for which the middleware responds
        /// </summary>
        public string Path { get; set; } = "api/_meta";

        /// <summary>
        /// Indent the metadata JSON response?
        /// </summary>
        public bool IndentJson { get; set; } = false;

        /// <summary>
        /// The API name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The API environment
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The API startup date and time
        /// </summary>
        public DateTimeOffset StartedOn { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// The API version
        /// </summary>
        public MetadataVersionOptions Version { get; set; } = new MetadataVersionOptions();
    }
}
