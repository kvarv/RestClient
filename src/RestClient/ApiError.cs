using System.Net;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Rest
{
    [DataContract(Name = "Error", Namespace = "")]
    [JsonObject(MemberSerialization.OptOut)]
    public class ApiError
    {
        [DataMember]
        [JsonProperty]
        public string ExceptionMessage { get; set; }

        [DataMember]
        [JsonProperty]
        public string ExceptionType { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        [DataMember]
        [JsonProperty]
        public string Message { get; set; }

        [DataMember]
        [JsonProperty]
        public string MessageDetail { get; set; }

        [DataMember]
        [JsonProperty]
        public string StackTrace { get; set; }
    }
}