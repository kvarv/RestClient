using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Rest
{
    public class JsonMediaTypeSerializer : IMediaTypeSerializer
    {
        private readonly List<string> _supportedMedaTypes;
        private JsonSerializer _jsonSerializer;

        public JsonMediaTypeSerializer()
        {
            _jsonSerializer = new JsonSerializer();
            _supportedMedaTypes = new List<string> { "application/json", "text/json" };
        }

        public IEnumerable<string> SupportedMedaTypes
        {
            get { return _supportedMedaTypes; }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                _jsonSerializer = new JsonSerializer();
                return _jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}