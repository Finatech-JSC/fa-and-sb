using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroBase.Share.Models
{
    public class BaseResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("messageCode")]
        public string MessageCode { get; set; }
        
        [JsonProperty("msgParams")]
        public List<string> MsgParams { get; set; }
        
        [JsonProperty("code")]
        public int Code { get; set; }
        
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }
        
        [JsonProperty("data")]
        public T Data { get; set; } = default;
    }
}