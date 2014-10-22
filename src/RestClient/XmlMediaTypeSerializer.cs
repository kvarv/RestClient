using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Rest
{
    public class XmlMediaTypeSerializer : IMediaTypeSerializer
    {
        private readonly List<string> _supportedMedaTypes;
        //concurrency issues??
        private readonly Dictionary<Type, DataContractSerializer> _cache = new Dictionary<Type, DataContractSerializer>(); 
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
            var type = typeof(T);
            DataContractSerializer serializer;
            if (_cache.ContainsKey(type))
            {
                serializer = _cache[type];
            }
            else
            {
                serializer = new DataContractSerializer(type);
                _cache.Add(type, serializer);
            }
            return (T)serializer.ReadObject(stream);
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