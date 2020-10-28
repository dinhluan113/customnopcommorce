using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.News;

namespace Nop.Plugin.Api.DTOs.Blogs
{
    public class NewsRootObjectDto : ISerializableObject
    {
        public NewsRootObjectDto()
        {
            News = new List<NewsDto>();
        }

        [JsonProperty("news")]
        public IList<NewsDto> News { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "news";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (NewsDto);
        }
    }
}