using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rest
{
    public class RestClient
    {
        private readonly HttpClient _httpClient;
        private readonly List<IMediaTypeSerializer> _mediaTypeSerializers;

        public RestClient(string baseUrl)
            : this(new HttpClient { BaseAddress = new Uri(baseUrl) })
        {
        }

        public RestClient(string baseUrl, HttpMessageHandler messageHandler)
            : this(new HttpClient(messageHandler) { BaseAddress = new Uri(baseUrl) })
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
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUri, UriKind.Relative));
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            using (var httpContent = response.Content)
            {
                if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var contentAsStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
                    var serializer = GetSerializerMatchingContentType(httpContent);
                    if (serializer == null)
                    {
                        throw new NotSupportedException("Deserialization of " + httpContent.Headers.ContentType.MediaType + " is not supported.");
                    }
                    return serializer.Deserialize<T>(contentAsStream);
                }

                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                {
                    var contentAsStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
                    var serializer = GetSerializerMatchingContentType(httpContent);
                    if (serializer == null)
                    {
                        throw new NotSupportedException("Deserialization of " + httpContent.Headers.ContentType.MediaType + " is not supported.");
                    }
                    var apiError = serializer.Deserialize<ApiError>(contentAsStream);
                    throw new ApiException(apiError);
                }

                if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiException(new ApiError { HttpStatusCode = response.StatusCode, Message = "Resource not found." });
                }
            }

            return default(T);
        }

        private IMediaTypeSerializer GetSerializerMatchingContentType(HttpContent httpContent)
        {
            return _mediaTypeSerializers.FirstOrDefault(mediaTypeSerializer => mediaTypeSerializer.SupportedMedaTypes.Any(mediatType => mediatType == httpContent.Headers.ContentType.MediaType));
        }
    }
}