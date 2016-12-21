using LocalStorage.Context;
using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToDoClient.Models;

namespace todoclient.Mappers
{
    public static class Mapper
    {
        public static ToDoItemViewModel TaskModel_To_ToDoViewModel(this TaskModel tm)
        {
            return new ToDoItemViewModel()
            {
                IsCompleted = tm.IsCompleted,
                Name = tm.Name,
                ToDoId = tm.Id,
                UserId = tm.User.Id
            };
        }

        public static TaskModel ToDoViewModel_To_TaskModel(this ToDoItemViewModel tm)
        {
            using (var db = new ToDoContext())
            {
                var model = new TaskModel();
                model.IsCompleted = tm.IsCompleted;
                model.Name = tm.Name;
                model.Id = tm.ToDoId;
                model.User = db.Users.FirstOrDefault(x => x.Id == tm.UserId);


                return model;
            }
        }
    }
}