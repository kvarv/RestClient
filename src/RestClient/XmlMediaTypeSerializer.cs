using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Rest
{
    public class XmlMediaTypeSerializer : IMediaTypeSerializer
    {
        private readonly List<string> _supportedMedaTypes;

        public XmlMediaTypeSerializer()
        {
            _supportedMedaTypes = new List<string> { "application/xml", "text/xml" };
        }

        public IEnumerable<string> SupportedMedaTypes
        {
            get { return _supportedMedaTypes; }
        }

        public T Deserialize<T>(Stream stream)
        {
            var deserializer = new DataContractSerializer(typeof(T));
            return (T)deserializer.ReadObject(stream);
        }

        public object Serialize(object body)
        {
            var serializer = new DataContractSerializer(body.GetType());
            var memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, body);
            return memoryStream;
        }
    }
}