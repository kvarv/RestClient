using System.Collections.Generic;
using System.IO;

namespace Rest.Client.Serializers
{
    public interface IMediaTypeSerializer
    {
        T Deserialize<T>(Stream stream);
        IEnumerable<string> SupportedMediaTypes { get; }
        object Serialize(object body);
    }
}