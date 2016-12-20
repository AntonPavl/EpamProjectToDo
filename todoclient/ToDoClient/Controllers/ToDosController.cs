using LocalStorage.Context;
using LocalStorage.Model;
using LocalStorage.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using todoclient.Mappers;
using ToDoClient.Models;
using ToDoClient.Services;

namespace ToDoClient.Controllers
{
    /// <summary>
    /// Processes todo requests.
    /// </summary>
    public class ToDosController : ApiController
    {
        private readonly ToDoService todoService = new ToDoService();
        private readonly UserService userService = new UserService();
        private readonly DataService dataService = new DataService();
        //Anton!!
        /// <summary>
        /// Returns all todo-items for the current user.
        /// </summary>
        /// <returns>The list of todo-items.</returns>
        public IList<ToDoItemViewModel> Get() //Work
        {
            var userId = dataService.GetOrCreateUser();
            return dataService.GetItems(userId).Select(x => x.TaskModel_To_ToDoViewModel()).ToList();
        }



        /// <summary>
        /// Updates the existing todo-item.
        /// </summary>
        /// <param name="todo">The todo-item to update.</param>
        public void Put(ToDoItemViewModel todo)
        {
            //todo.UserId = userService.GetOrCreateUser(); 
            //todoService.UpdateItem(todo); 
            todo.UserId = dataService.GetOrCreateUser();
            dataService.UpdateItem(todo.ToDoViewModel_To_TaskModel());
        }

        /// <summary>
        /// Deletes the specified todo-item.
        /// </summary>
        /// <param name="id">The todo item identifier.</param>
        public void Delete(int id)
        {
            // todoService.DeleteItem(id); 
            dataService.DeleteItem(id);
        }



        /// <summary>
        /// Creates a new todo-item.
        /// </summary>
        /// <param name="todo">The todo-item to create.</param>
        public void Post(ToDoItemViewModel todo) //Work
        {
            todo.UserId = dataService.GetOrCreateUser();
            dataService.CreateItem(todo.ToDoViewModel_To_TaskModel());
        }
    }
}
