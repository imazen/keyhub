using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KeyHub.Core.Settings
{
    /// <summary>
    /// Provides runtime access to all settings components
    /// </summary>
    public class SettingsContext : ISettingsContext
    {
        #region "Singleton"

        public SettingsContext()
        {
            Initialize();
        }

        #endregion "Singleton"

        #region "Settings classes"

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        private IEnumerable<ISettingsFile> settings { get; set; }

        /// <summary>
        /// Gets the settings file for the given class
        /// </summary>
        /// <typeparam name="TSetting">The type of the settings file to get</typeparam>
        /// <returns>A prefilled settings file</returns>
        public TSetting GetSetting<TSetting>() where TSetting : ISettingsFile
        {
            // Query the settings list to get the correct setting
            return (TSetting)(from x in settings
                              where x is TSetting
                              select x).FirstOrDefault();
        }

        #endregion "Settings classes"

        #region "Initialization"

        /// <summary>
        /// Initializes all settings and writes XML if neccesary.
        /// </summary>
        private void Initialize()
        {
            var newSettings = new List<ISettingsFile>();

            foreach (var setting in settings)
            {
                // Generate the settings path for this settings class
                var settingsPath = GetSettingsPath(setting);

                // Generate or read the settings file and add it to the new settings collection
                if (System.IO.File.Exists(settingsPath))
                {
                    var settingsContent = ReadSettingsFile(settingsPath);
                    newSettings.Add(Common.Serializers.Utils.XmlSerializers.SerializeXmlToClass<ISettingsFile>(settingsContent, setting.GetType()));
                }
                else
                {
                    WriteSettingsFile(settingsPath, Common.Serializers.Utils.XmlSerializers.SerializeClassToXml(setting));
                    newSettings.Add(setting);
                }
            }

            // Save the collection by assiging the Settings collection
            settings = newSettings;
        }

        #endregion "Initialization"

        #region "IO helpers"

        /// <summary>
        /// Generates a file path for a settings file
        /// </summary>
        /// <param name="settingsFile">The settings file instance</param>
        /// <returns>The file path for the settings file</returns>
        private string GetSettingsPath(ISettingsFile settingsFile)
        {
            return System.Web.HttpContext.Current.Server.MapPath(string.Format("~/Config/{0}", settingsFile.FileName));
        }

        /// <summary>
        /// Writes the settings file to disk
        /// </summary>
        /// <param name="settingsPath">The path to the settings file</param>
        /// <param name="settingsContent">The settings XML content</param>
        private void WriteSettingsFile(string settingsPath, string settingsContent)
        {
            System.IO.File.WriteAllText(settingsPath, settingsContent, Encoding.UTF8);
        }

        /// <summary>
        /// Reads the settings file from disk
        /// </summary>
        /// <param name="settingsPath">The path to the settings file</param>
        /// <returns>XML content from the settings file</returns>
        private string ReadSettingsFile(string settingsPath)
        {
            return System.IO.File.ReadAllText(settingsPath, Encoding.UTF8);
        }

        #endregion "IO helpers"
    }
}