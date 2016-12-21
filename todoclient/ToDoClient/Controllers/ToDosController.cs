using LocalStorage.Context;
using LocalStorage.Interfaces;
using LocalStorage.Model;
using LocalStorage.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using todoclient.Mappers;
using ToDoClient.Models;

namespace ToDoClient.Controllers
{
    /// <summary>
    /// Processes todo requests.
    /// </summary>
    public class ToDosController : ApiController
    {
        private readonly IDataService dataService = new DataService();

        /// <summary>
        /// Returns all todo-items for the current user.
        /// </summary>
        /// <returns>The list of todo-items.</returns>
        public IList<ToDoItemViewModel> Get() //Work
        {
            var userId = dataService.GetOrCreateUser();
           // var l = dataService.GetItems(userId).ToList();
            var f = dataService.GetItems(userId).Select(x => x.TaskModel_To_ToDoViewModel()).ToList();
            return f;
        }
        /// <summary>
        /// Updates the existing todo-item.
        /// </summary>
        /// <param name="todo">The todo-item to update.</param>
        public void Put(ToDoItemViewModel todo)
        {
            todo.UserId = dataService.GetOrCreateUser();
            var updateData = dataService.GetItems(todo.UserId).ToList().FirstOrDefault(x => x.Name == todo.Name);
            updateData.IsCompleted = todo.IsCompleted;
            dataService.UpdateItem(updateData);
        }

        /// <summary>
        /// Deletes the specified todo-item.
        /// </summary>
        /// <param name="id">The todo item identifier.</param>
        public void Delete(int id)
        {
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
