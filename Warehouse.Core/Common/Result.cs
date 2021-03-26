using System;
using System.Net;

namespace Warehouse.Core.Common
{
    public class Result<T> : Exception
    {
        public string Field { get; }
        public override string Message { get; }
        public new T Data { get; }
        public HttpStatusCode StatusCode { get; }

        private Result(T data, string field, string message, HttpStatusCode statusCode)
        {
            Data = data;
            Field = field;
            Message = message;
            StatusCode = statusCode;
        }


        /// <summary>
        /// Returns success result from business logic
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Data</returns>
        public static Result<T> Success(T data = default)
        {
            return new(data, string.Empty, string.Empty, default);
        }

        /// <summary>
        /// Returns failure result from business logic
        /// </summary>
        /// <param name="field"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns>Errors</returns>
        public static Result<T> Failure(string field = "", string message = "", HttpStatusCode statusCode = default)
        {
            return new(default, field, message, statusCode);
        }
    }
}