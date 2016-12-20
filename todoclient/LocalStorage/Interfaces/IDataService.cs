using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage.Interfaces
{
    public interface IDataService
    {
        IList<TaskModel> GetItems(int userId);
        int GetOrCreateUser();
        void UpdateItem(TaskModel todo);
        void CreateItem(TaskModel todo);
        void DeleteItem(int id);
    }
}
