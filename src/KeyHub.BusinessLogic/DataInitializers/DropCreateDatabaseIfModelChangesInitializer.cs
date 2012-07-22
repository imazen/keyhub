using System;
using System.Data.Entity;
using KeyHub.Model;
using KeyHub.Runtime;

namespace KeyHub.BusinessLogic.DataInitializers
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