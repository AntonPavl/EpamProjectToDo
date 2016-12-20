using LocalStorage.Model;
using LocalStorage.Context;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System;

namespace LocalStorage.Repository
{
    public class DataRepository
    {
        private List<UserModel> users = new List<UserModel>();
        public DataRepository()
        {
        }

        public int CreateUser(int id)
        {
            using (var db = new DbContext())
            {
                var user = new UserModel() { Id = id };
                db.Users.Add(user);
                db.SaveChanges();
            }
            return 1;
            //return id;
        }
        public IList<TaskModel> GetItems(int userId)
        {
            using (var db = new DbContext())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                var tasks = db.Tasks.Where(x => x.User.Id == userId);
                return tasks.ToList() ;
            }
        }
        public void UpdateItem(TaskModel todo)
        {
            using (var db = new DbContext())
            {
                var item = db.Tasks.FirstOrDefault(x => x.Id == todo.Id);
                if (item != null)
                {

                    item.IsCompleted = todo.IsCompleted;
                    item.Name = todo.Name;
                    item.User = todo.User;
                    db.SaveChanges();
                }
            }
        }
        public void CreateItem(TaskModel todo)
        {
            using (var db = new DbContext())
            {
                var f = db.Users.FirstOrDefault(x => x.Id == todo.User.Id);
                var model = new TaskModel()
                {
                    Id = todo.Id,
                    IsCompleted = todo.IsCompleted,
                    Name = todo.Name
                };
                f.TaskList.Add(model);
                //db.Tasks.Add(todo);
                db.SaveChanges();
            }
        }

        public void DeleteItem(int id)
        {
            using (var db = new DbContext())
            {
                db.Tasks.Remove(db.Tasks.FirstOrDefault( x => x.Id == id));
            }
        }

    }
}
