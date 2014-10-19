using System;
using System.Net.Http;

namespace Rest
{
    public class RestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient(string url)
            : this(new HttpClient {BaseAddress = new Uri(url)})
        {
        }

        public RestClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }
    }
}