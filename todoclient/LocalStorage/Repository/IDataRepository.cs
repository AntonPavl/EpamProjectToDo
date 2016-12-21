using System.Collections.Generic;
using LocalStorage.Model;

namespace LocalStorage.Repository
{
    public interface IDataRepository
    {
        int CreateUser(int id);
        UserModel GetUserById(int id);
        IList<TaskModel> GetItems(int userId);
        TaskModel GetItemById(int id);
        TaskModel UpdateItem(TaskModel todo);
        TaskModel CreateItem(TaskModel todo, int userId);
        void DeleteItem(int id);
    }
}