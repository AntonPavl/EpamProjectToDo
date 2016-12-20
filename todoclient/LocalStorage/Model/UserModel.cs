using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage.Model
{
    public class UserModel
    {
        public UserModel()
        {
            TaskList = new List<TaskModel>();
        }
        public int Id { get; set; }
        public virtual List<TaskModel> TaskList { get; set; }
    }
}
