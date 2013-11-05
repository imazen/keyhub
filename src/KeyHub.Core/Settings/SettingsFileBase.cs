using System;
using System.Xml.Serialization;

namespace KeyHub.Core.Settings
{
    /// <summary>
    /// Provides basic functionality for settings files
    /// </summary>
    [Serializable]
    public abstract class SettingsFileBase : ISettingsFile
    {
        /// <summary>
        /// Gets the root node of the settings file for the XML
        /// </summary>
        [XmlIgnore]
        public abstract string RootNode
        {
            get;
        }

        /// <summary>
        /// Gets the filename of the settings file
        /// </summary>
        [XmlIgnore]
        public abstract string FileName
        {
            get;
        }

        /// <summary>
        /// Empty constructor for serializer support
        /// </summary>
        public SettingsFileBase()
        {
        }
    }
}