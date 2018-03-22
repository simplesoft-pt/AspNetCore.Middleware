namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// The metadata version model
    /// </summary>
    public class MetadataVersionModel
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        /// <param name="revision"></param>
        /// <param name="alias"></param>
        public MetadataVersionModel(uint major, uint? minor = null, uint? patch = null, uint? revision = null, string alias = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Alias = alias;
        }

        /// <summary>
        /// Major version
        /// </summary>
        public uint Major { get; }

        /// <summary>
        /// Minor version
        /// </summary>
        public uint? Minor { get; }

        /// <summary>
        /// Patch version
        /// </summary>
        public uint? Patch { get; }

        /// <summary>
        /// Revision version
        /// </summary>
        public uint? Revision { get; }

        /// <summary>
        /// Version alias
        /// </summary>
        public string Alias { get; }
    }
}