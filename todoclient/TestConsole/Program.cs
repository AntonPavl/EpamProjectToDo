using LocalStorage.Context;
using LocalStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new DbContext())
            {
                var user1 = new UserModel() {Id=29 };
                db.Users.Add(user1);
                db.SaveChanges();

                var users = db.Users.ToList();
                Console.WriteLine("Список объектов:");
                foreach (var u in users)
                {
                    Console.WriteLine("{0}.{1}", u.Id, u.TaskList);
                }
            }
            Console.Read();
        }
    }
}
