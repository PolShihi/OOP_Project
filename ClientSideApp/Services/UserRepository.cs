using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ISettingsService _settingsService;

        public UserRepository(ISettingsService settingsService)
        {
            _client = new HttpClient();
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _serializerSettings.Formatting = Formatting.Indented;
            _settingsService = settingsService;
        }

        private async Task Authorize()
        {
            string? token = await SecureStorage.GetAsync("Token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<ApiResponse<string>> CreateAccount(RegisterDTO registerDTO)
        {
            await Authorize();

            string requestJson = JsonConvert.SerializeObject(registerDTO, _serializerSettings);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage? response;
            try
            {
                response = await _client.PostAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account/register", content);
            }
            catch(HttpRequestException)
            {
                return ApiResponse<string>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse<string>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<string>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<string>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }
            
            return ApiResponse<string>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<string>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.DeleteAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<string>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<string>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<string>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<string>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<string>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<UserSession?>> FirstOrDefaultAsync(Func<UserSession, bool> filter, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<UserSession?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<UserSession>>>(responseJson);
                var userSession = apiResponse.Data.FirstOrDefault(filter);
                return ApiResponse<UserSession?>.SuccessResponse(userSession);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<UserSession?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<UserSession?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<UserSession?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<UserSession?>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<UserSession?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<UserSession?>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<UserSession?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<UserSession?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<UserSession?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<List<UserSession>>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<List<UserSession>>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<List<UserSession>>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<List<UserSession>>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<List<UserSession>>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<List<UserSession>>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<List<UserSession>>> ListAsync(Func<UserSession, bool> filter, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<List<UserSession>>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<UserSession>>>(responseJson);
                var userSessions = apiResponse.Data.Where(filter).ToList();
                return ApiResponse<List<UserSession>>.SuccessResponse(userSessions);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<List<UserSession>>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<List<UserSession>>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<List<UserSession>>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginAccount(LoginDTO loginDTO)
        {
            string requestJson = JsonConvert.SerializeObject(loginDTO, _serializerSettings);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage? response;
            try
            {
                response = await _client.PostAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account/login", content);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<LoginResponseDTO>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse<LoginResponseDTO>>(responseJson);
            }

            return ApiResponse<LoginResponseDTO>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<string>> UpdateAsync(string id, RegisterDTO user, CancellationToken cancellationToken = default)
        {
            await Authorize();

            string requestJson = JsonConvert.SerializeObject(user, _serializerSettings);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage? response;
            try
            {
                response = await _client.PutAsync($"https://{_settingsService.Host}:{_settingsService.Port}/api/Account/{id}", content, cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<string>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest || 
                response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.NotFound)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<string>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<string>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<string>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<string>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }
    }
}
