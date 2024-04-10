using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientSideApp.Models;
using Newtonsoft.Json.Serialization;

namespace ClientSideApp.Services
{
    public class UserService : IUserService
    {
        public async Task<LoginResponse> Authenticate(LoginRequest loginRequest)
        {
            using (var client = new HttpClient())
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                string loginRequestStr = JsonConvert.SerializeObject(loginRequest, Formatting.Indented, settings);

                var response = await client.PostAsync($"https://{App.Host}:{App.Port}/api/Account/login",
                      new StringContent(loginRequestStr, Encoding.UTF8,
                      "application/json"));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LoginResponse>(json);
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<UserInfo>> GetUsers()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://{App.Host}:{App.Port}/api/Account");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserInfo>>(json);
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task Register(RegistrationRequest registrationRequest)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await SecureStorage.GetAsync("Token"));
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                string registrationRequestStr = JsonConvert.SerializeObject(registrationRequest, Formatting.Indented, settings);

                var response = await client.PostAsync($"https://{App.Host}:{App.Port}/api/Account/register",
                      new StringContent(registrationRequestStr, Encoding.UTF8,
                      "application/json"));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        public async Task DeleteUser(string id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await SecureStorage.GetAsync("Token"));

                var response = await client.DeleteAsync($"https://{App.Host}:{App.Port}/api/Account/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
        }
    }

}
