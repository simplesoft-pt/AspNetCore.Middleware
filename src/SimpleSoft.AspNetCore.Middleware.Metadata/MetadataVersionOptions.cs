namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Version information
    /// </summary>
    public class MetadataVersionOptions
    {
        /// <summary>
        /// Major version
        /// </summary>
        public uint Major { get; set; }

        /// <summary>
        /// Minor version
        /// </summary>
        public uint Minor { get; set; }

        /// <summary>
        /// Patch version
        /// </summary>
        public uint Patch { get; set; }

        /// <summary>
        /// Revision version
        /// </summary>
        public uint Revision { get; set; }

        /// <summary>
        /// Version alias. Defaults to '0.0.0'
        /// </summary>
        public string Alias { get; set; } = "0.0.0";
    }
}