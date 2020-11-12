using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.News;

namespace Nop.Plugin.Api.DTOs.Blogs
{
    public class TopicRootObjectDto : ISerializableObject
    {
        public TopicRootObjectDto()
        {
        }

        [JsonProperty("topic")]
        public TopicDto Topic { get; set; }


        public string GetPrimaryPropertyName()
        {
            return "topic";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (NewsDto);
        }
    }
}