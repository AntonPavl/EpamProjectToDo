using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage.Interfaces
{
    public interface IDataRepository
    {
        int CreateUser(int id);
        IList<TaskModel> GetItems(int userId);
        void UpdateItem(TaskModel todo);
        TaskModel CreateItem(TaskModel todo);
        TaskModel DeleteItem(int id);
        void CreateItems(IList<TaskModel> list);
    }
}
