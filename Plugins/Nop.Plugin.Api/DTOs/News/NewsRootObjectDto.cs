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

        [JsonProperty("total")]
        public int TotalItems { get; set; }

        [JsonProperty("remain")]
        public int RemainItems { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

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