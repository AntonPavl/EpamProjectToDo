using LocalStorage.Context;
using LocalStorage.Delete;
using LocalStorage.Model;
using LocalStorage.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace LocalStorage.Service
{
    public class DataService
    {
        private readonly IDataRepository _repository = new DataRepository();

        private const string ServiceApiUrl = "http://todo-service-api.azurewebsites.net/api/";
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;
        private readonly RemoteToDoService _remoteToDoService;

        public DataService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _userService = new UserService(_httpClient, ServiceApiUrl);
            _remoteToDoService = new RemoteToDoService(ServiceApiUrl);
        }

        public UserService UserService
        {
            get
            {
                return _userService;
            }
        }

        public IList<TaskModel> GetItems(int userId)
        {
            var localItems = _repository.GetItems(userId).ToList();
            var remoteItems = _remoteToDoService.GetAllTodoFromRemote(_httpClient, userId);

            SyncTasks(localItems, remoteItems);

            return _repository.GetItems(userId);
        }

        
        public void UpdateItem(TaskModel todo)
        {
            var item =_repository.UpdateItem(todo);
            var t = new Thread(() => _remoteToDoService.SendPutMess(_httpClient, item));
            t.Start();
        }

        public TaskModel CreateItem(TaskModel todo, int userId)
        {
            var item = _repository.CreateItem(todo, userId);

            var t = new Thread(() => _remoteToDoService.SendPostMess(_httpClient, item));
            t.Start();

            return item;
        }

        public void DeleteItem(int id, int userId)
        {
            var localTask = _repository.GetItemById(id);

            if (localTask == null)
            {
                return;
            }

            var task = new TaskModel
            {
                ToDoId = localTask.ToDoId,
                Name = localTask.Name,
                IsCompleted = localTask.IsCompleted
            };

            _repository.DeleteItem(id);

            try
            {
                new Thread(() => DeleteTaskFromRemote(task, userId)).Start();
            }
            catch
            {
                // ignore it....
            }
        }

        private void UpdateTaskOnRemote(TaskModel taskModel, int userId)
        {
            if (taskModel.ToDoId.HasValue == false)
            {
                var remoteTasks = _remoteToDoService.GetAllTodoFromRemote(_httpClient, userId);

                var equalTasks = remoteTasks.Where(r => r.Name.Trim() == taskModel.Name);

                if (equalTasks.Any() == false)
                {
                    return;
                }

                var lastEqualTaskId = equalTasks.Last().ToDoId;
                taskModel.ToDoId = lastEqualTaskId;
            }
            
            _remoteToDoService.SendPutMess(_httpClient, taskModel);
        }

        private void DeleteTaskFromRemote(TaskModel task, int userId)
        {
            if (task.ToDoId.HasValue)
            {
                _remoteToDoService.SendDeleteMess(_httpClient, task.ToDoId.Value);
            }
            else
            {
                var remoteTasks = _remoteToDoService.GetAllTodoFromRemote(_httpClient, userId);

                var equalTasks = remoteTasks.Where(r => r.Name.Trim() == task.Name && r.IsCompleted == task.IsCompleted);

                if (equalTasks.Any() == false)
                {
                    return;
                }

                var lastEqualTaskId = equalTasks.Last().ToDoId;
                _remoteToDoService.SendDeleteMess(_httpClient, lastEqualTaskId);
            }
        }

        private void SyncTasks(List<TaskModel> localTasks, List<RemoteTaskModel> remoteTasks)
        {
            // remove from local
            foreach (var localTask in localTasks)
            {
                if (localTask.ToDoId.HasValue == false)
                {
                    _repository.DeleteItem(localTask.Id);
                    continue;
                }

                var remoteTask = remoteTasks.FirstOrDefault(r => r.ToDoId == localTask.ToDoId);

                if (remoteTask == null)
                {
                    _repository.DeleteItem(localTask.Id);
                }
            }

            // add task from remote
            foreach (var remoteTask in remoteTasks)
            {
                var localTask = localTasks.FirstOrDefault(r => r.ToDoId == remoteTask.ToDoId);

                if (localTask == null)
                {
                    var task = new TaskModel
                    {
                        ToDoId = remoteTask.ToDoId,
                        IsCompleted = remoteTask.IsCompleted,
                        Name = remoteTask.Name,
                        User = new UserModel { Id = remoteTask.UserId }
                    };

                    _repository.CreateItem(task, remoteTask.UserId);
                }
            }
        }
    }
}
