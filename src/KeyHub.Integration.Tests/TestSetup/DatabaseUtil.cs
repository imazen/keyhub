using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;

namespace KeyHub.Integration.Tests.TestSetup
{
    class DatabaseUtil
    {
        public static TItem InsertItem<TItem>(DataContext dataContext, IDbSet<TItem> dbSet, TItem item) where TItem : class
        {
            dbSet.Add(item);
            dataContext.SaveChanges();
            return item;
        }
    }
}
