using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Rest
{
    public class JsonMediaTypeSerializer : IMediaTypeSerializer
    {
        private readonly List<string> _supportedMedaTypes;
        private readonly JsonSerializer _jsonSerializer;

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
                return _jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }

        public object Serialize(object body)
        {
            return JsonConvert.SerializeObject(body);
        }
    }
}