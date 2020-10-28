using Nop.Core.Domain.News;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs.News;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class NewsDtoMapping
    {
        public static NewsDto ToDto(this NewsItem news)
        {
            NewsDto dto = new NewsDto();
            dto.Id = news.Id;
            dto.Title = news.Title;
            dto.Short = news.Short;
            dto.Full = news.Full;
            dto.MetaDescription = news.MetaDescription;
            dto.MetaKeywords = news.MetaKeywords;
            dto.MetaTitle = news.MetaTitle;
            dto.CreatedOnUtc = news.CreatedOnUtc;
            dto.URL = news.Title.ToURL() + "-" + news.Id;
            return dto;
        }
    }
}
