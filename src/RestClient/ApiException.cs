using System;

namespace Rest
{
    public class ApiException : Exception
    {
        public ApiError ApiError { get; private set; }

        public ApiException(ApiError apiError)
        {
            ApiError = apiError;
        }
    }
}