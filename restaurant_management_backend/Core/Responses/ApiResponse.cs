using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Responses
{
    public class ApiResponse<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "")
        {
            return new ApiResponse<T> { Data = data, Success = true, Message = message };
        }
        public static ApiResponse<T> Ok(string message = "")
        {
            return new ApiResponse<T> { Data = null, Success = true, Message = message };
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T> { Data = null, Success = false, Message = message };
        }
    }
}
