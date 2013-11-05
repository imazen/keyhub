namespace KeyHub.Web.Composition
{
    /// <summary>
    /// Provides access to all commenly used objects
    /// </summary>
    public static class ObjectManager
    {
        public static ObjectFactory ObjectFactory { get; private set; }
        
        public static void StartApplication(ObjectFactory objectFactory)
        {
            ObjectFactory = objectFactory;
        }
    }
}