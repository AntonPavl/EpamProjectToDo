using LocalStorage.Context;
using LocalStorage.Delete;
using LocalStorage.Interfaces;
using LocalStorage.Model;
using LocalStorage.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LocalStorage.Service
{
    public class DataService : IDataService
    {
        private static IDataRepository _repository = new DataRepository();

        private const string GetAllUrl = "ToDos?userId={0}";
        private const string UpdateUrl = "ToDos";
        private const string CreateUrl = "Users";
        private const string CreateUrlTodo = "ToDos";
        private const string DeleteUrl = "ToDos/{0}";
        private const string serviceApiUrl = "http://todo-service-api.azurewebsites.net/api/";
        private HttpClient httpClient;
        public DataService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        /// <summary>
        /// Get all todos
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<TaskModel> GetItems(int userId)
        {
            var t = new Thread(() => GetServerItems(userId));
            t.Start();
            var ret = _repository.GetItems(userId);
            return ret;
        }
        /// <summary>
        /// Get user from cookies if exists  else create new user instance
        /// </summary>
        /// <returns></returns>
        public int GetOrCreateUser()
        {
            var userCookie = HttpContext.Current.Request.Cookies["user"];
            int userId;
            if (userCookie == null || !Int32.TryParse(userCookie.Value, out userId))
            {
                userId = CreateUser("Noname: " + 1);
                var cookie = new HttpCookie("user", userId.ToString())
                {
                    Expires = DateTime.Today.AddMonths(1)
                };

                HttpContext.Current.Response.SetCookie(cookie);
            }
            return userId;
        }

        private int CreateUser(string userName)
        {
            var response = httpClient.PostAsJsonAsync(serviceApiUrl + CreateUrl, userName).Result;
            response.EnsureSuccessStatusCode();
            var userid = response.Content.ReadAsAsync<int>().Result;

            _repository.CreateUser(userid);
            return 1;
            //return userid;
        }
        /// <summary>
        /// Update todo
        /// </summary>
        /// <param name="todo"></param>
        public void UpdateItem(TaskModel todo)
        {
            if (todo != null)
            {
                _repository.UpdateItem(todo);
                var data = todo.TaskModel_To_ToDoViewModel();
                data.ToDoId = todo.RealId;
                var t = new Thread(() => SendPutMess(httpClient, todo));
                t.Start();
            }
        }
        /// <summary>
        /// create todo
        /// </summary>
        /// <param name="todo"></param>
        public void CreateItem(TaskModel todo)
        {
            _repository.CreateItem(todo);
            var t = new Thread(() => SendPostMess(httpClient,todo));
            t.Start();
        }
        /// <summary>
        /// Delete todo
        /// </summary>
        /// <param name="id"></param>
        public void DeleteItem(int id)
        {
            var model = _repository.DeleteItem(id);
            var t = new Thread(() => SendDeleteMess(httpClient, model.RealId));
            t.Start();
        }


        private static void SendPostMess(HttpClient httpClient, TaskModel todo) //возможно ошибка будет е*ли на *ервере де*ереализация
        {
            httpClient.PostAsJsonAsync(serviceApiUrl + CreateUrlTodo, todo.TaskModel_To_ToDoViewModel()).Result.EnsureSuccessStatusCode();
        }
        private static void SendPutMess(HttpClient httpClient, TaskModel todo)
        {
            var test = todo.TaskModel_To_ToDoViewModel();
            test.ToDoId = test.RealId;
            try
            {
                httpClient.PutAsJsonAsync(serviceApiUrl + UpdateUrl, test).Result.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                ///Change ViewModel to Model without RealId
            }
        }
        private static void SendDeleteMess(HttpClient httpClient, int id)
        {
            try
            {
                httpClient.DeleteAsync(string.Format(serviceApiUrl + DeleteUrl, id)).Result.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                SendDeleteMess(httpClient, id);
            }
        }
        /// <summary>
        /// Get all todos from server and save real todo id
        /// </summary>
        /// <param name="userId"></param>
        private void GetServerItems(int userId)
        {
            var dataAsString = httpClient.GetStringAsync(string.Format(serviceApiUrl + GetAllUrl, userId)).Result;
            var itemlist = JsonConvert.DeserializeObject<IList<ToDoItemViewModel>>(dataAsString).Select(x => x.ToDoViewModel_To_TaskModel()).ToList();
            if (itemlist != null) _repository.CreateItems(itemlist);
        }
    }
}
