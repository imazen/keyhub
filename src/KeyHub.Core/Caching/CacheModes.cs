namespace KeyHub.Core.Caching
{
    /// <summary>
    /// Defines the different cache modes
    /// </summary>
    public enum CacheModes
    {
        /// <summary>
        /// Cache will expire no matter what the last access timestamp is
        /// </summary>
        Absolute = 1,

        /// <summary>
        /// Every time the cache item is accessed, the timer will reset
        /// </summary>
        Sliding = 2
    }
}