using LocalStorage.Context;
using LocalStorage.Delete;
using LocalStorage.Model;
using LocalStorage.Repository;
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
    public class DataService
    {
        private static DataRepository _repository = new DataRepository();

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
        public IList<TaskModel> GetItems(int userId)
        {
            return _repository.GetItems(userId);
        }

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

        public void UpdateItem(TaskModel todo)
        {
            _repository.UpdateItem(todo);
            var t = new Thread(() => SendPutMess(httpClient, todo));
            t.Start();
        }

        public void CreateItem(TaskModel todo)
        {
            _repository.CreateItem(todo);
            var t = new Thread(() => SendPostMess(httpClient,todo));
            t.Start();
        }
        public void DeleteItem(int id)
        {
            _repository.DeleteItem(id);
            var t = new Thread(() => SendDeleteMess(httpClient, id));
            t.Start();
        }


        private static void SendPostMess(HttpClient httpClient, TaskModel todo)
        {
            httpClient.PostAsJsonAsync(serviceApiUrl + CreateUrlTodo, todo.TaskModel_To_ToDoViewModel()).Result.EnsureSuccessStatusCode();
        }
        private static void SendPutMess(HttpClient httpClient, TaskModel todo)
        {
            httpClient.PutAsJsonAsync(serviceApiUrl + UpdateUrl, todo.TaskModel_To_ToDoViewModel()).Result.EnsureSuccessStatusCode();
        }
        private static void SendDeleteMess(HttpClient httpClient, int id)
        {
            httpClient.DeleteAsync(string.Format(serviceApiUrl + DeleteUrl, id)).Result.EnsureSuccessStatusCode();
        }
    }
}
