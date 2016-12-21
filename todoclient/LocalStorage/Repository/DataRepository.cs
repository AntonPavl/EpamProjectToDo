using LocalStorage.Model;
using LocalStorage.Context;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System;

namespace LocalStorage.Repository
{
    public class DataRepository : IDisposable, IDataRepository
    {
        private readonly ToDoContext _todoContext;

        public DataRepository()
        {
            _todoContext = new ToDoContext();
        }

        public int CreateUser(int id)
        {
            var user = new UserModel() { Id = id };
            _todoContext.Users.Add(user);
            _todoContext.SaveChanges();

            // wtf???
            return 1;
            //return id;
        }

        public UserModel GetUserById(int id)
        {
            UserModel user;

            user = _todoContext.Users.FirstOrDefault(u => u.Id == id);

            return user;
        }


        public IList<TaskModel> GetItems(int userId)
        {
            var user = _todoContext.Users.FirstOrDefault(x => x.Id == userId);
            var tasks = _todoContext.Tasks.Where(x => x.User.Id == userId);
            return tasks.ToList();
        }

        public TaskModel UpdateItem(TaskModel todo)
        {
            var item = _todoContext.Tasks.FirstOrDefault(x => x.Id == todo.Id);
            if (item != null)
            {
                item.IsCompleted = todo.IsCompleted;
                item.Name = todo.Name;
                item.User = _todoContext.Users.FirstOrDefault(x=>x.Id == todo.User.Id);
                _todoContext.SaveChanges();
            }

            return item;
        }
        public TaskModel CreateItem(TaskModel todo, int userId)
        {
            var user = _todoContext.Users.FirstOrDefault(x => x.Id == userId);

            var model = new TaskModel()
            {
                Id = todo.Id,
                IsCompleted = todo.IsCompleted,
                Name = todo.Name,
                ToDoId = todo.ToDoId,
                //User = user
            };

            user.TaskList.Add(model);
            //_todoContext.Tasks.Add(todo);
            _todoContext.SaveChanges();

            return model;
        }

        public void DeleteItem(int id)
        {
            var item = _todoContext.Tasks.FirstOrDefault(x => x.Id == id);
            _todoContext.Tasks.Remove(item);

            _todoContext.SaveChanges();
        }

        public TaskModel GetItemById(int id)
        {
            return _todoContext.Tasks.FirstOrDefault(r => r.Id == id);
        }

        public void Dispose()
        {
            _todoContext.Dispose();
        }
    }
}
