using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data) => 
            new ApiResponse<T> { Success = true, Data = data, StatusCode = 200 };
        public static ApiResponse<T> ErrorResponse(string errorMessage, int statusCode, List<string>? errors = null) => 
            new ApiResponse<T> { Success = false, ErrorMessage = errorMessage, StatusCode = statusCode, Errors = errors ?? new() };
    }
}
