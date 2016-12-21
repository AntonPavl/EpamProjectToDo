﻿using LocalStorage.Context;
using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage.Delete
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
                return new TaskModel()
                {
                    IsCompleted = tm.IsCompleted,
                    Name = tm.Name,
                    Id = tm.ToDoId,
                    User = db.Users.FirstOrDefault(x => x.Id == tm.UserId)
                };
            }
        }
    }
}
