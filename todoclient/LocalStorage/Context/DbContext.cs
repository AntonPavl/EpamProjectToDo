using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage.Context
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext() : base("DbConnection")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }
        public virtual DbSet<TaskModel> Tasks { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }
    }
}
