using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Rest.Tests.TestRestServer;
using Should;
using Xunit;

namespace Rest.Tests
{
    public class Tests
    {
        [Fact]
        public async Task Should_get_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos");

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public async Task Should_get_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos");

                foos.Count.ShouldEqual(3);
            }
        }
    }
}