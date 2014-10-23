using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Rest.Tests.TestRestServer
{
    [RoutePrefix("api/foos")]
    public class FooController : ApiController
    {
        [Route("{id}")]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CreateFoo());
        }

        [Route("")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new List<Foo> { CreateFoo(), CreateFoo(), CreateFoo() });
        }

        [Route("")]
        public HttpResponseMessage Get(string param1, string param2)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CreateFoo());
        }

        [Route("")]
        public HttpResponseMessage Post(Foo foo)
        {
            return Request.CreateResponse(HttpStatusCode.Created, 1);
        }

        [Route("")]
        public HttpResponseMessage Post(string param1, string param2, Foo foo)
        {
            return Request.CreateResponse(HttpStatusCode.Created, 1);
        }

        [Route("{id}")]
        public HttpResponseMessage Put(int id, Foo foo)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("")]
        public HttpResponseMessage Put(string param1, string param2, Foo foo)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{id}")]
        public HttpResponseMessage Patch(int id, Foo foo)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("")]
        public HttpResponseMessage Patch(string param1, string param2, Foo foo)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{id}")]
        public HttpResponseMessage Delete(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("")]
        public HttpResponseMessage Delete(string param1, string param2)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static Foo CreateFoo()
        {
            return new Foo
            {
                SomeInt = 1,
                SomeString = "some",
                Bar = new Bar
                {
                    SomeInt = 1,
                    SomeString = "some"
                }
            };
        }
    }

    public class Foo
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
        public Bar Bar { get; set; }
    }

    public class Bar
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
    }
}