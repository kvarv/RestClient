using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rest
{
    public class RestClient
    {
        private readonly HttpClient _httpClient;
        private readonly List<IMediaTypeSerializer> _mediaTypeSerializers;

        public RestClient(string baseUrl)
            : this(new HttpClient {BaseAddress = new Uri(baseUrl)})
        {
        }

        public RestClient(string baseUrl, HttpMessageHandler messageHandler)
            : this(new HttpClient(messageHandler) {BaseAddress = new Uri(baseUrl)})
        {
        }

        public RestClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _mediaTypeSerializers = new List<IMediaTypeSerializer>
            {
                new JsonMediaTypeSerializer(),
                new XmlMediaTypeSerializer()
            };
        }

        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }

        public List<IMediaTypeSerializer> MediaTypeSerializers
        {
            get { return _mediaTypeSerializers; }
        }

        public async Task<T> GetAsync<T>(string requestUri)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUri, UriKind.Relative));
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

            using (var httpContent = httpResponseMessage.Content)
            {
                var contentAsStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
                var serializer = MediaTypeSerializers.FirstOrDefault(mediaTypeSerializer => mediaTypeSerializer.SupportedMedaTypes.Any(mediatType => mediatType == httpContent.Headers.ContentType.MediaType));
                if (serializer == null)
                {
                    throw new NotSupportedException("Deserialization of " + httpContent.Headers.ContentType.MediaType + " is not supported.");
                }
                return serializer.Deserialize<T>(contentAsStream);
            }
        }
    }
}