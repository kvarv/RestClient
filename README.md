RestClient
==========
A library that maskes it easy to create a client over your ASP.NET Web Api 2 rest endpoint.

## Convetions

This library assumes that you return *HttpResponseMessage* from your ApiController actions.

`return Request.CreateResponse(HttpStatusCode.OK, composition.CompositionVersion.Data);`


### Returning errors/exceptions
The *RestClient* asumes that you use

`return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);`

if you want to return an Exception/Error. This will return an instance of *HttpError* which is automatically
deserialized and thrown through an *ApiException*. If an unhandled exception is thrown, Web Api will create an
*HttpError* for you.

## Serialization

*RestClient* serializes request content based on the content type. It supports *Json* and *Bson* through Json.Net and *Xml*
through DataContractSerializer.

*RestClient* desrializes based on the content type of the returned response.

## Custom serialization

If you want to do custom serialization/deserialization you should create a media type for your particular case
and implement *IMediaTypeSerializer* in your client. This can then be plugged in with

`MediaTypeSerializers.Add(new OpenEhrResultSetXmlMediaTypeSerializer());`

## More?

For now, look at the unit tests :)
