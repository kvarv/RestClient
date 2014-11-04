using System;
using System.Net;

namespace Rest.Client
{
    public class ApiException : Exception
    {
        public ApiError ApiError { get; private set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public ApiException(ApiError apiError)
        {
            ApiError = apiError;
        }
    }
}