using System.ComponentModel.Composition;

namespace KeyHub.Core.Settings
{
    /// <summary>
    /// Represents a settings file. All values must have a default value to include them in the XML output file
    /// </summary>
    [InheritedExport(typeof(ISettingsFile))]
    public interface ISettingsFile
    {
        /// <summary>
        /// Gets the root node of the settings file for the XML
        /// </summary>
        string RootNode { get; }

        /// <summary>
        /// Gets the filename of the settings file
        /// </summary>
        string FileName { get; }
    }
}