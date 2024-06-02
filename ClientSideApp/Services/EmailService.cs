using MyModel.Models.DTOs;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Threading;
using System.Net.Http.Headers;

namespace ClientSideApp.Services
{
    public class EmailService : IEmailService
    {
        protected readonly HttpClient _client;
        protected readonly JsonSerializerSettings _serializerSettings;
        protected readonly ISettingsService _settingsService;

        public EmailService(ISettingsService settingsService)
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
            return $"https://{_settingsService.Host}:{_settingsService.Port}/api/Email";
        }

        public async Task<ApiResponse<string>> SendEmailAsync(EmailSendRequestDTO emailSendRequest, CancellationToken cancellationToken = default)
        {
            await Authorize();

            string requestJson = JsonConvert.SerializeObject(emailSendRequest, _serializerSettings);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage? response;
            try
            {
                response = await _client.PostAsync($"{GetApiURI()}/send", content, cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<string>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
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
