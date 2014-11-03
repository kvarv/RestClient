using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Rest.Tests.TestRestServer;
using Should;
using Xunit;

namespace Rest.Tests
{
    public class BsonTests
    {
        [Fact]
        public async Task Should_get_bson_list()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos", MediaTypes.ApplicationBson);

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public async Task Should_get_bson()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<Foo>("/api/foos/{0}".FormatUri(1), MediaTypes.ApplicationBson);

                foos.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_get_bson_simple_type()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var parameters = new Dictionary<string, string> { { "param", "a_param" } };
                var foo = await restClient.GetAsync<int>("/api/foos", null, parameters);

                foo.ShouldNotEqual(0);
            }
        }

        [Fact]
        public async Task Should_post_bson()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), MediaTypes.ApplicationJson, MediaTypes.ApplicationJson);

                id.ShouldEqual(1);
            }
        }
    }
}