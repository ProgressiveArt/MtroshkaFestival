using System;

namespace MetroshkaFestival.Core.Exceptions.Common
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(in int statusCode, string message)
        {
            Status = statusCode;
            Value = message;
        }

        public HttpResponseException(in int statusCode)
        {
            Status = statusCode;
        }

        public int Status { get; set; }

        public object Value { get; set; }
    }
}