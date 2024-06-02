using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace ClientSideApp.Services
{
    public class OrderRepository : Repository<Order, OrderDTO>
    {
        public OrderRepository(ISettingsService settingsService) : base(settingsService)
        {
        }

        public virtual async Task<ApiResponse<Order?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await Authorize();

            HttpResponseMessage? response;
            HttpResponseMessage? responsePrice;
            try
            {
                response = await _client.GetAsync($"{GetApiURI()}/{id}", cancellationToken);
                responsePrice = await _client.GetAsync($"{GetApiURI()}/{id}/Price", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return ApiResponse<Order?>.ErrorResponse("There was no response from the server. Check your connection.", 0);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var responsePriceJson = await responsePrice.Content.ReadAsStringAsync(cancellationToken);
                var responseApi = JsonConvert.DeserializeObject<ApiResponse<Order?>>(responseJson);
                var responsePriceApi = JsonConvert.DeserializeObject<ApiResponse<decimal?>>(responseJson);
                responseApi.Data.TotalPrice = responsePriceApi.Data.Value;
                return responseApi;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<ApiResponse<Order?>>(responseJson);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponse<Order?>.ErrorResponse("You are unauthorized.", (int)response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponse<Order?>.ErrorResponse("You dont have permission.", (int)response.StatusCode);
            }

            return ApiResponse<Order?>.ErrorResponse("Unknown error.", (int)response.StatusCode);
        }
    }
}
