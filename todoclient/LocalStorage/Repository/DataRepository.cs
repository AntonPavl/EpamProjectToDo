using LocalStorage.Model;
using LocalStorage.Context;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System;
using LocalStorage.Interfaces;

namespace LocalStorage.Repository
{
    public class DataRepository : IDataRepository
    {
        private List<UserModel> users = new List<UserModel>();
        public DataRepository()
        {
        }
        /// <summary>
        /// create user in repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get all todos from repository
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<TaskModel> GetItems(int userId)
        {
            using (var db = new DbContext())
            {
                var tasks = db.Tasks.Where(x => x.User.Id == userId);
                return tasks.ToList() ;
            }
        }
        /// <summary>
        /// Update todo in repositry
        /// </summary>
        /// <param name="todo"></param>
        public void UpdateItem(TaskModel todo)
        {
            using (var db = new DbContext())
            {
                var item = db.Tasks.FirstOrDefault(x => x.Id == todo.Id);
                if (item != null)
                {

                    item.IsCompleted = todo.IsCompleted;
                    item.Name = todo.Name;
                    item.User = db.Users.FirstOrDefault(x=>x.Id == todo.User.Id);
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// create and save Todo
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public TaskModel CreateItem(TaskModel todo)
        {
            TaskModel ret;
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
                ret = model;
                db.SaveChanges();
            }
            return ret;
        }
        /// <summary>
        /// Delete todo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskModel DeleteItem(int id)
        {
            TaskModel ret;
            using (var db = new DbContext())
            {
                ret = db.Tasks.Remove(db.Tasks.FirstOrDefault( x => x.Id == id));
                db.SaveChanges();
            }
            return ret;
        }
        /// <summary>
        /// create a lot of items and save
        /// </summary>
        /// <param name="list"></param>
        public void CreateItems(IList<TaskModel> list)
        {
            if (list!= null)
            {
                using (var db = new DbContext())
                {
                    var f = db.Users.FirstOrDefault(x => x.Id == list[0].User.Id);
                    foreach (var item in list)
                    {
                        if (db.Tasks.Contains(item))
                        {
                            var task = db.Tasks.FirstOrDefault(x => x.Name == item.Name);
                            task.RealId = item.RealId;
                        }
                        else
                        {
                            var model = new TaskModel()
                            {
                                RealId = item.Id,
                                IsCompleted = item.IsCompleted,
                                Name = item.Name
                            };
                            f.TaskList.Add(model);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}
