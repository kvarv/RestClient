using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Rest.Serializers;

namespace Rest
{
    public class RestClient : IRestClient
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
                                        new XmlMediaTypeSerializer(),
                                        new BsonMediaTypeSerializer()
                                    };
        }

        public HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
        }

        public List<IMediaTypeSerializer> MediaTypeSerializers
        {
            get
            {
                return _mediaTypeSerializers;
            }
        }

        public Task DeleteAsync(string requestUri, string accept, IDictionary<string, string> parameters = null)
        {
            var uri = new Uri(requestUri, UriKind.Relative).ApplyParameters(parameters);
            return SendAsync<object>(uri, HttpMethod.Delete, null, null, accept);
        }

        public async Task<T> GetAsync<T>(string requestUri, string accept, IDictionary<string, string> parameters = null)
        {
            var uri = new Uri(requestUri, UriKind.Relative).ApplyParameters(parameters);
            return await SendAsync<T>(uri, HttpMethod.Get, null, null, accept);
        }

        public Task<T> PatchAsync<T>(string requestUri, object body, string contentType, string accept, IDictionary<string, string> parameters = null)
        {
            var uri = new Uri(requestUri, UriKind.Relative).ApplyParameters(parameters);
            var response = SendAsync<T>(uri, HttpVerb.Patch, body, contentType, accept);
            return response;
        }

        public Task<T> PostAsync<T>(string requestUri, object body, string contentType, string accept, IDictionary<string, string> parameters = null)
        {
            var uri = new Uri(requestUri, UriKind.Relative).ApplyParameters(parameters);
            return SendAsync<T>(uri, HttpMethod.Post, body, contentType, accept);
        }

        public Task<T> PutAsync<T>(string requestUri, object body, string contentType, string accept, IDictionary<string, string> parameters = null)
        {
            var uri = new Uri(requestUri, UriKind.Relative).ApplyParameters(parameters);
            var response = SendAsync<T>(uri, HttpMethod.Put, body, contentType, accept);
            return response;
        }

        private HttpRequestMessage CreateRequest(Uri uri, HttpMethod httpMethod, object body, string contentType, string accept)
        {
            var request = new HttpRequestMessage(httpMethod, uri);
            if (accept != null)
            {
                request.Headers.Add("Accept", accept);
            }

            if (body != null)
            {
                var httpContent = body as HttpContent;

                if (httpContent != null)
                {
                    request.Content = httpContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
                else
                {
                    var serializer = GetSerializerMatchingContentType(contentType);
                    var content = serializer.Serialize(body);

                    var stringContent = content as string;
                    if (stringContent != null)
                    {
                        request.Content = new StringContent(stringContent, Encoding.UTF8, contentType);
                    }

                    var streamContent = content as Stream;
                    if (streamContent != null)
                    {
                        request.Content = new StreamContent(streamContent);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    }
                }
            }

            return request;
        }

        private IMediaTypeSerializer GetSerializerMatchingContentType(string mediaType)
        {
            var serializer = _mediaTypeSerializers.FirstOrDefault(mediaTypeSerializer => mediaTypeSerializer.SupportedMediaTypes.Any(mediatType => mediatType == mediaType));
            if (serializer == null)
            {
                throw new NotSupportedException("Deserialization of " + mediaType + " is not supported.");
            }

            return serializer;
        }

        private async Task<T> SendAsync<T>(Uri uri, HttpMethod httpMethod, object body, string contentType, string accept)
        {
            var request = CreateRequest(uri, httpMethod, body, contentType, accept);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.Content != null)
            {
                using (var httpContent = response.Content)
                {
                    if (response.IsSuccessStatusCode && httpContent.Headers.ContentType != null)
                    {
                        var contentAsStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
                        var serializer = GetSerializerMatchingContentType(httpContent.Headers.ContentType.MediaType);
                        return serializer.Deserialize<T>(contentAsStream);
                    }

                    if (!response.IsSuccessStatusCode && httpContent.Headers.ContentType != null)
                    {
                        var contentAsStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
                        var serializer = GetSerializerMatchingContentType(httpContent.Headers.ContentType.MediaType);
                        var apiError = serializer.Deserialize<ApiError>(contentAsStream);
                        throw new ApiException(apiError) { HttpStatusCode = response.StatusCode, ReasonPhrase = response.ReasonPhrase };
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ApiException(new ApiError { Message = response.ReasonPhrase }) { HttpStatusCode = response.StatusCode, ReasonPhrase = response.ReasonPhrase };
                    }
                }
            }

            return default(T);
        }
    }
}