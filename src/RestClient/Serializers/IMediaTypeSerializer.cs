using System.Collections.Generic;
using System.IO;

namespace Rest.Serializers
{
    public interface IMediaTypeSerializer
    {
        T Deserialize<T>(Stream stream);
        IEnumerable<string> SupportedMedaTypes { get; }
        object Serialize(object body);
    }
}