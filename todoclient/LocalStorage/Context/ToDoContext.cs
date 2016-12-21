using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace LocalStorage.Context
{
    public class ToDoContext : DbContext
    {
        public ToDoContext() : base("DbConnection")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }
        public virtual DbSet<TaskModel> Tasks { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }
    }
}
