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
                var tasks = db.Tasks.Where(x => x.UserId == userId);
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
                    item.UserId = db.Users.FirstOrDefault(x=>x.Id == todo.UserId).Id;
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
                var f = db.Users.FirstOrDefault(x => x.Id == todo.UserId);
                var model = new TaskModel()
                {
                    Id = todo.Id,
                    IsCompleted = todo.IsCompleted,
                    Name = todo.Name,
                    UserId = todo.UserId

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
                    var index = 1; //test
                    var f = db.Users.FirstOrDefault(x => x.Id == index);
                    foreach (var item in list)
                    {
                        var task = db.Tasks.FirstOrDefault(x => x.Name == item.Name);
                        if (task != null)
                        {
                            task.RealId = item.Id;
                        }
                        else
                        {
                            var model = new TaskModel()
                            {
                                RealId = item.Id,
                                IsCompleted = item.IsCompleted,
                                Name = item.Name,
                                UserId = item.UserId
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
