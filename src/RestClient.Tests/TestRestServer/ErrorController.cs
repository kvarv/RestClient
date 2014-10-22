using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Rest.Tests.TestRestServer
{
    [RoutePrefix("api/error")]
    public class ErrorController : ApiController
    {
        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(id));
        }
    }
}