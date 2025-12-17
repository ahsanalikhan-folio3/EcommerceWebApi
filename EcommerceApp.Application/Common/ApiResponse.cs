using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Application.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? Errors { get; set; }

        public ApiResponse(bool Success, string? Message, object? Data = null, object? Errors = null)
        {
            this.Success = Success;
            this.Message = Message;
            this.Data = Data;
            this.Errors = Errors;
        }

        public static ApiResponse SuccessResponse (string? Message, object? Data)
        {
            return new ApiResponse(true, Message, Data: Data);
        }

        public static ApiResponse ErrorResponse (string? Message, object? Errors)
        {
            return new ApiResponse(false, Message, Errors: Errors);
        }
    }
}
