using System;
using System.Data.Entity;
using KeyHub.Model;

namespace KeyHub.Data.Initializers
{
#if DEBUG

    /// <summary>
    /// If the model has changed, the database will be deleted and recreated. DEBUG only
    /// </summary>
    public class DropCreateDatabaseIfModelChangesInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
    }

#endif
}