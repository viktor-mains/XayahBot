﻿using Newtonsoft.Json;

namespace XayahBot.API.Error
{
    public class Status
    {
        [JsonProperty("Status_Code")]
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
