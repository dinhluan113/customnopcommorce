using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Blogs;
using Nop.Plugin.Api.DTOs.Blogs;
using Nop.Services.News;
using Nop.Plugin.Api.DTOs.News;

namespace Nop.Plugin.Api.Controllers
{
    public class NewsController : BaseApiController
    {
        private readonly IUrlRecordService _urlRecordService;
        private readonly INewsService _newsService;
        private readonly IFactory<Product> _factory;
        private readonly IDTOHelper _dtoHelper;

        public NewsController(IJsonFieldsSerializer jsonFieldsSerializer,
                                  INewsService newsService,
                                  IUrlRecordService urlRecordService,
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  IFactory<Product> factory,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _factory = factory;
            _urlRecordService = urlRecordService;
            _newsService = newsService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Retrieve all blog
        /// </summary>
        /// <param name="pageSize">size: 16</param>
        /// <param name="pageIndex">index: 1</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        public IHttpActionResult GetNews(int pageSize = 16, int pageIndex = 1, string fields = "")
        {
            var news = _newsService.GetAllNews(pageSize: pageSize, pageIndex: pageIndex, showHidden: true);

            if (news == null)
            {
                return Error(HttpStatusCode.NotFound, "news", "not found");
            }

            IList<NewsDto> newsAsDtos = news.Select(c =>
            {
                return _dtoHelper.PrepareNewsDTO(c);
            }).ToList();

            var total = _newsService.GetAllNews().Count;
            var remain = total - (pageIndex + 1) * pageSize;
            var remainPage = Math.Ceiling((double)(total / pageSize) / 10);

            var newsRootObject = new NewsRootObjectDto()
            {
                News = newsAsDtos,
                TotalItems = total,
                RemainItems = remain,
                TotalPages = Convert.ToInt32(remainPage) + 1
            };

            var json = _jsonFieldsSerializer.Serialize(newsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve all news by id
        /// </summary>
        /// <param name="id">size: 16</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        public IHttpActionResult GetNewsById(int id, string fields = "")
        {
            var news = _newsService.GetNewsById(id);

            if (news == null)
            {
                return Error(HttpStatusCode.NotFound, "news", "not found");
            }

            var newsAsDtos = new List<NewsDto>() { _dtoHelper.PrepareNewsDTO(news) };

            var newsRootObject = new NewsRootObjectDto()
            {
                News = newsAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(newsRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}