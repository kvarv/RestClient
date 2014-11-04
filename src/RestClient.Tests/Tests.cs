using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Owin.Testing;

using Rest.Client.Serializers;
using Rest.Client.Tests.TestRestServer;

using Should;

using Xunit;

namespace Rest.Client.Tests
{
    public class Tests
    {
        [Fact]
        public async Task Should_get_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos", MediaTypes.ApplicationJson);

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public async Task Should_get_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos", MediaTypes.ApplicationXml);

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public void Should_handle_error_when_getting_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                const string ErrorMessage = "error";

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/error/" + ErrorMessage, MediaTypes.ApplicationJson).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.Message.ShouldEqual(ErrorMessage);
            }
        }

        [Fact]
        public void Should_handle_error_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                const string ErrorMessage = "error";

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/error/" + ErrorMessage, MediaTypes.ApplicationXml).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.Message.ShouldEqual(ErrorMessage);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/unknown", MediaTypes.ApplicationJson).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/unknown", MediaTypes.ApplicationXml).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_throw_exception_for_unkown_server_mediatype()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var aggregateException = Assert.Throws<AggregateException>(() =>
                {
                    var result = restClient.GetAsync<List<Foo>>("/api/foos", "application/foos").Result;
                    return result;
                });

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
            }
        }

        [Fact]
        public void Should_throw_exception_for_unkown_client_mediatype()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.MediaTypeSerializers.Remove(restClient.MediaTypeSerializers.First(x => x is JsonMediaTypeSerializer));

                var aggregateException = Assert.Throws<AggregateException>(() =>
                {
                    var result = restClient.GetAsync<List<Foo>>("/api/foos", MediaTypes.ApplicationJson).Result;
                    return result;
                });

                aggregateException.InnerException.ShouldBeType<NotSupportedException>();
            }
        }

        [Fact]
        public async Task Should_get_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };
                var foo = await restClient.GetAsync<Foo>("/api/foos", null, parameters);

                foo.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_get_with_id()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foo = await restClient.GetAsync<Foo>("/api/foos/{0}".FormatUri(1), null);

                foo.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_post_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), MediaTypes.ApplicationJson, MediaTypes.ApplicationJson);

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_post_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), MediaTypes.ApplicationXml, MediaTypes.ApplicationXml);

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_post_form_url_encoded_content()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var values = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("SomeInt", "1") };
                var content = new FormUrlEncodedContent(values);

                var id = await restClient.PostAsync<int>("/api/foos", content, MediaTypes.FormUrlEncoded, null);

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_post_json_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), MediaTypes.ApplicationJson, null, parameters);

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_put_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var obj = await restClient.PutAsync<object>("/api/foos/{0}".FormatUri(1), new Foo(), MediaTypes.ApplicationJson, null);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_put_json_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };

                var obj = await restClient.PutAsync<object>("/api/foos", new Foo(), MediaTypes.ApplicationJson, null, parameters);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_put_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var obj = await restClient.PutAsync<object>("/api/foos/{0}".FormatUri(1), new Foo(), MediaTypes.ApplicationXml, null);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_put_form_url_encoded_content()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var values = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("SomeInt", "1") };
                var content = new FormUrlEncodedContent(values);

                var obj = await restClient.PutAsync<object>("/api/foos/{0}".FormatUri(1), content, MediaTypes.FormUrlEncoded, null);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_delete_with_id()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                await restClient.DeleteAsync("/api/foos/{0}".FormatUri(1), null);
            }
        }

        [Fact]
        public async Task Should_delete_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };

                await restClient.DeleteAsync("/api/foos", null, parameters);
            }
        }

        [Fact]
        public async Task Should_patch_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var obj = await restClient.PatchAsync<object>("/api/foos/{0}".FormatUri(1), new Foo(), MediaTypes.ApplicationJson, null);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_patch_json_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };

                var obj = await restClient.PatchAsync<object>("/api/foos", new Foo(), MediaTypes.ApplicationJson, null, parameters);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_patch_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var obj = await restClient.PatchAsync<object>("/api/foos/{0}".FormatUri(1), new Foo(), MediaTypes.ApplicationXml, null);

                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_patch_form_url_encoded_content()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var values = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("SomeInt", "1") };
                var content = new FormUrlEncodedContent(values);

                var obj = await restClient.PatchAsync<object>("/api/foos/{0}".FormatUri(1), content, MediaTypes.FormUrlEncoded, null);

                obj.ShouldBeNull();
            }
        }
    }
}