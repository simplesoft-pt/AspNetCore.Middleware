﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Metadata middleware options
    /// </summary>
    public class MetadataOptions
    {
        private static readonly string DefaultName;
        private static readonly DateTimeOffset DefaultStartedOn;
        private static readonly MetadataVersionOptions DefaultVersionOptions;

        static MetadataOptions()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            DefaultName = entryAssembly.GetName().Name;
            DefaultStartedOn = DateTimeOffset.Now;

            try
            {
                var fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);

                DefaultVersionOptions = new MetadataVersionOptions
                {
                    Major = (uint) fvi.FileMajorPart,
                    Minor = (uint) fvi.FileMinorPart,
                    Patch = (uint) fvi.FileBuildPart,
                    Revision = (uint) fvi.FilePrivatePart,
                    Alias = fvi.ProductVersion
                };
            }
            catch (Exception)
            {
                DefaultVersionOptions = new MetadataVersionOptions();
            }
        }

        /// <summary>
        /// Path for which the middleware responds
        /// </summary>
        public string Path { get; set; } = "api/_meta";

        /// <summary>
        /// Indent the JSON response?
        /// </summary>
        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// The API name
        /// </summary>
        public string Name { get; set; } = DefaultName;

        /// <summary>
        /// The API environment
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The API startup date and time
        /// </summary>
        public DateTimeOffset StartedOn { get; set; } = DefaultStartedOn;

        /// <summary>
        /// The API version
        /// </summary>
        public MetadataVersionOptions Version { get; set; } = new MetadataVersionOptions
        {
            Major = DefaultVersionOptions.Major,
            Minor = DefaultVersionOptions.Minor,
            Patch = DefaultVersionOptions.Patch,
            Revision = DefaultVersionOptions.Revision,
            Alias = DefaultVersionOptions.Alias
        };
    }
}
