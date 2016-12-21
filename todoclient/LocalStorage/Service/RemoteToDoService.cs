using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LocalStorage.Delete;
using LocalStorage.Model;
using Newtonsoft.Json;

namespace LocalStorage.Service
{
    internal class RemoteToDoService
    {
        private const string GetAllUrl = "ToDos?userId={0}";
        private const string UpdateUrl = "ToDos";
        private const string CreateUrlTodo = "ToDos";
        private const string DeleteUrl = "ToDos/{0}";
        private readonly string _serviceApiUrl;

        public RemoteToDoService(string serviceApiUrl)
        {
            _serviceApiUrl = serviceApiUrl;
        }

        public void SendPostMess(HttpClient httpClient, TaskModel todo)
        {
            var result =
                httpClient.PostAsJsonAsync(_serviceApiUrl + CreateUrlTodo, todo.TaskModel_To_ToDoViewModel()).Result;

            result.EnsureSuccessStatusCode();
        }
        public void SendPutMess(HttpClient httpClient, TaskModel todo)
        {
            var viewModel = todo.TaskModel_To_ToDoViewModel();
            viewModel.ToDoId = todo.ToDoId.GetValueOrDefault();
            httpClient.PutAsJsonAsync(_serviceApiUrl + UpdateUrl, viewModel).Result.EnsureSuccessStatusCode();
        }

        public void SendDeleteMess(HttpClient httpClient, int id)
        {
            try
            {
                httpClient.DeleteAsync(string.Format(_serviceApiUrl + DeleteUrl, id)).Result.EnsureSuccessStatusCode();
            }
            catch
            {
                // ignore
            }
        }

        public List<RemoteTaskModel> GetAllTodoFromRemote(HttpClient httpClient, int userId)
        {
            var request = (HttpWebRequest)WebRequest.Create(_serviceApiUrl + string.Format(GetAllUrl, userId));
            List<RemoteTaskModel> taskModels;

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var responseContent = streamReader.ReadToEnd();
                    taskModels = JsonConvert.DeserializeObject<List<RemoteTaskModel>>(responseContent ?? string.Empty);
                }
            }

            return taskModels;
        }
    }
}
