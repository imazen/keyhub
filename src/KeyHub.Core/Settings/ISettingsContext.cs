namespace KeyHub.Core.Settings
{
    public interface ISettingsContext
    {
        /// <summary>
        /// Gets the settings file for the given class
        /// </summary>
        /// <typeparam name="TSetting">The type of the settings file to get</typeparam>
        /// <returns>A prefilled settings file</returns>
        TSetting GetSetting<TSetting>() where TSetting : ISettingsFile;
    }
}