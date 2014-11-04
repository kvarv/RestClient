using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Rest.Client.Serializers
{
    public class XmlMediaTypeSerializer : IMediaTypeSerializer
    {
        private readonly List<string> m_supportedMediaTypes;
        //concurrency issues??
        private readonly Dictionary<Type, DataContractSerializer> _cache = new Dictionary<Type, DataContractSerializer>(); 
        public XmlMediaTypeSerializer()
        {
            m_supportedMediaTypes = new List<string> { MediaTypes.ApplicationXml, MediaTypes.TextXml };
        }

        public IEnumerable<string> SupportedMediaTypes
        {
            get { return m_supportedMediaTypes; }
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
            var type = body.GetType();
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

            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, body);
                return memoryStream;   
            }
        }
    }
}