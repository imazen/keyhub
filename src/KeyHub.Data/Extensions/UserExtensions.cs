using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data
{
    public static class UserExtensions
    {
        public static User GetByUserName(this DbSet<User> userSet, string userName)
        {
            return userSet.FirstOrDefault(x => x.UserName == userName);
        }
    }
}
