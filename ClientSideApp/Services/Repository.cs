using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;

namespace ClientSideApp.Services
{
    public class Repository<EntityType, DTO> : IRepository<EntityType, DTO>
        where DTO : class
        where EntityType : Entity
    {
        protected readonly HttpClient _client;
        protected readonly JsonSerializerSettings _serializerSettings;
        protected readonly ISettingsService _settingsService;

        public Repository(ISettingsService settingsService)
        {
            _client = new HttpClient();
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _serializerSettings.Formatting = Formatting.Indented;
            _settingsService = settingsService;
        }

        protected async Task Authorize()
        {
            string? token = await SecureStorage.GetAsync("Token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected string GetApiURI()
        {
            return $"https://{_settingsService.Host}:{_settingsService.Port}/api/{typeof(EntityType).Name}";
        }

        public async Task<ApiResponse<EntityType?>> AddAsync(DTO dto, CancellationToken cancellationToken = default)
        {
            await Authorize();

            string requestJson = JsonConvert.SerializeObject(dto, _serializerSettings);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage? response;
            try
            {
                response = await _client.PostAsync($"{GetApiURI()}", content, cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<EntityType?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<EntityType?>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<EntityType?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.DeleteAsync($"{GetApiURI()}/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<string>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
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

        public async Task<ApiResponse<EntityType?>> FirstOrDefaultAsync(Func<EntityType, bool> filter, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"{GetApiURI()}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<EntityType?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<EntityType>>>(responseJson);
                var entity = apiResponse.Data.FirstOrDefault(filter);
                return ApiResponse<EntityType?>.SuccessResponse(entity);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<EntityType?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public virtual async Task<ApiResponse<EntityType?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"{GetApiURI()}/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<EntityType?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<EntityType?>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<EntityType?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<List<EntityType>>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"{GetApiURI()}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<List<EntityType>>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<List<EntityType>>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<List<EntityType>>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<List<EntityType>>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<List<EntityType>>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<List<EntityType>>> ListAsync(Func<EntityType, bool> filter, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            try
            {
                response = await _client.GetAsync($"{GetApiURI()}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<List<EntityType>>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<EntityType>>>(responseJson);
                var entities = apiResponse.Data.Where(filter).ToList();
                return ApiResponse<List<EntityType>>.SuccessResponse(entities);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<List<EntityType>>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<List<EntityType>>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<List<EntityType>>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }

        public async Task<ApiResponse<EntityType?>> UpdateAsync(int id, DTO dto, CancellationToken cancellationToken = default)
        {
            await Authorize();

            string requestJson = JsonConvert.SerializeObject(dto, _serializerSettings);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage? response;
            try
            {
                response = await _client.PutAsync($"{GetApiURI()}/{id}", content, cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<EntityType?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<EntityType?>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<EntityType?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<EntityType?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }
    }
}
