using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Rest.Serializers
{
    public class BsonMediaTypeSerializer : IMediaTypeSerializer
    {
        private readonly List<string> m_supportedMediaTypes;
        private readonly JsonSerializer _jsonSerializer;

        public BsonMediaTypeSerializer()
        {
            _jsonSerializer = new JsonSerializer();
            m_supportedMediaTypes = new List<string> { MediaTypes.ApplicationBson };
        }

        public IEnumerable<string> SupportedMediaTypes
        {
            get { return m_supportedMediaTypes; }
        }

        public T Deserialize<T>(Stream stream)
        {
            var bsonReader = new BsonReader(stream);

            if (typeof (IEnumerable).IsAssignableFrom(typeof (T)))
            {
                bsonReader.ReadRootValueAsArray = true;
            }

            using (bsonReader)
            {
                return _jsonSerializer.Deserialize<T>(bsonReader);
            }
        }

        public object Serialize(object body)
        {
            using(var memoryStream = new MemoryStream())
            using (var bsonWriter = new BsonWriter(memoryStream))
            {
                _jsonSerializer.Serialize(bsonWriter, body);
                return memoryStream;   
            }
        }
    }
}