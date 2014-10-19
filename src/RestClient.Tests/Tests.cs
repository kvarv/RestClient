using System.Threading.Tasks;
using Xunit;

namespace Rest.Tests
{
    public class Tests
    {
        [Fact]
        public async Task Should()
        {
            var restClient = new RestClient("");

            var httpResponseMessage = await restClient.HttpClient.GetAsync("");
            
        }
    }
}