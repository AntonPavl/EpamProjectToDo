using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LocalStorage.Repository;

namespace LocalStorage.Service
{
    public class UserService
    {
        private readonly IDataRepository _repository = new DataRepository();
        private readonly HttpClient _httpClient;
        private readonly string _serviceApiUrl;
        private const string CreateUrl = "Users";

        public UserService(HttpClient httpClient, string serviceApiUrl)
        {
            _httpClient = httpClient;
            _serviceApiUrl = serviceApiUrl;
        }

        public int GetOrCreateUser()
        {
            var userCookie = HttpContext.Current.Request.Cookies["user"];
            int userId = -1;

            if (userCookie == null)
            {
                userId = AddCookieAndCreateUser();
            }
            else if (int.TryParse(userCookie.Value, out userId) == false)
            {
                userId = AddCookieAndCreateUser();
            }
            // if use doesn't exist in DB
            else if (_repository.GetUserById(userId) == null)
            {
                userId = AddCookieAndCreateUser();
            }

            return userId;
        }

        private int CreateUser(string userName)
        {
            var response = _httpClient.PostAsJsonAsync(_serviceApiUrl + CreateUrl, userName).Result;
            response.EnsureSuccessStatusCode();
            var userid = response.Content.ReadAsAsync<int>().Result;

            _repository.CreateUser(userid);
            return 1;
            //return userid;
        }

        private int AddCookieAndCreateUser()
        {
            var userId = CreateUser("Noname: " + 1);
            var cookie = new HttpCookie("user", userId.ToString())
            {
                Expires = DateTime.Today.AddMonths(1)
            };

            HttpContext.Current.Response.SetCookie(cookie);

            return userId;
        }
    }
}
