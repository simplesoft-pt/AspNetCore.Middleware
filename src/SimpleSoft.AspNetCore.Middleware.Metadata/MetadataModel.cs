using System;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// The medatata model
    /// </summary>
    public class MetadataModel
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="environment"></param>
        /// <param name="startedOn"></param>
        /// <param name="version"></param>
        public MetadataModel(string name, string environment, DateTimeOffset? startedOn = null, MetadataVersionModel version = null)
        {
            Name = name;
            Environment = environment;
            StartedOn = startedOn;
            Version = version;
        }

        /// <summary>
        /// The API name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The API environment
        /// </summary>
        public string Environment { get; }

        /// <summary>
        /// The API startup date and time
        /// </summary>
        public DateTimeOffset? StartedOn { get; }

        /// <summary>
        /// The API version
        /// </summary>
        public MetadataVersionModel Version { get; }
    }
}