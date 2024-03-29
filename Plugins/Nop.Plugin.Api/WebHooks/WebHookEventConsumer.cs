﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.WebHooks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Services;
using Nop.Services.Events;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Catalog;
using Nop.Services.Stores;
using Nop.Core.Domain.Messages;
using Nop.Core.Caching;

namespace Nop.Plugin.Api.WebHooks
{
    public class WebHookEventConsumer : IConsumer<EntityInserted<Customer>>,
        IConsumer<EntityUpdated<Customer>>,
        IConsumer<EntityInserted<Product>>,
        IConsumer<EntityUpdated<Product>>,
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityInserted<Order>>,
        IConsumer<EntityUpdated<Order>>,
        IConsumer<EntityInserted<StoreMapping>>,
        IConsumer<EntityDeleted<StoreMapping>>,
        IConsumer<EntityInserted<GenericAttribute>>,
        IConsumer<EntityUpdated<GenericAttribute>>,
        IConsumer<EntityUpdated<Store>>,
        IConsumer<EntityInserted<ProductCategory>>,
        IConsumer<EntityUpdated<ProductCategory>>,
        IConsumer<EntityDeleted<ProductCategory>>,
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        IConsumer<EntityInserted<ProductPicture>>,
        IConsumer<EntityUpdated<ProductPicture>>,
        IConsumer<EntityDeleted<ProductPicture>>,
        IConsumer<EntityUpdated<Picture>>,
        IConsumer<EntityInserted<NewsLetterSubscription>>,
        IConsumer<EntityUpdated<NewsLetterSubscription>>,
        IConsumer<EntityDeleted<NewsLetterSubscription>>
    {
        private readonly IWebHookManager _webHookManager;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IProductPictureService _productPictureService;
        private IStoreService _storeService;
        private IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;

        private IDTOHelper _dtoHelper;

        public WebHookEventConsumer(IStoreService storeService)
        {
            IWebHookService webHookService = EngineContext.Current.ContainerManager.Resolve<IWebHookService>();
            _customerApiService = EngineContext.Current.ContainerManager.Resolve<ICustomerApiService>();
            _categoryApiService = EngineContext.Current.ContainerManager.Resolve<ICategoryApiService>();
            _productApiService = EngineContext.Current.ContainerManager.Resolve<IProductApiService>();
            _dtoHelper = EngineContext.Current.ContainerManager.Resolve<IDTOHelper>();
            _storeService = EngineContext.Current.ContainerManager.Resolve<IStoreService>();
            _productPictureService = EngineContext.Current.ContainerManager.Resolve<IProductPictureService>();

            _productService = EngineContext.Current.ContainerManager.Resolve<IProductService>();
            _categoryService = EngineContext.Current.ContainerManager.Resolve<ICategoryService>();
            _storeMappingService = EngineContext.Current.ContainerManager.Resolve<IStoreMappingService>();
            _storeContext = EngineContext.Current.ContainerManager.Resolve<IStoreContext>();

            _cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

            _webHookManager = webHookService.GetHookManager();
        }

        public void HandleEvent(EntityInserted<Customer> eventMessage)
        {
            // There is no need to send webhooks for guest customers.
            if (eventMessage.Entity.IsGuest())
            {
                return;
            }

            CustomerDto customer = _customerApiService.GetCustomerById(eventMessage.Entity.Id);
            var storeIds = new List<int>();

            if (customer.RegisteredInStoreId.HasValue)
            {
                storeIds.Add(customer.RegisteredInStoreId.Value);
            }

            NotifyRegisteredWebHooks(customer, WebHookNames.CustomersCreate, storeIds);
        }

        public void HandleEvent(EntityUpdated<Customer> eventMessage)
        {
            // There is no need to send webhooks for guest customers.
            if (eventMessage.Entity.IsGuest())
            {
                return;
            }

            CustomerDto customer = _customerApiService.GetCustomerById(eventMessage.Entity.Id, true);

            // In nopCommerce the Customer, Product, Category and Order entities are not deleted.
            // Instead the Deleted property of the entity is set to true.
            string webhookEvent = WebHookNames.CustomersUpdate;

            if (customer.Deleted == true)
            {
                webhookEvent = WebHookNames.CustomersDelete;
            }

            var storeIds = new List<int>();

            if (customer.RegisteredInStoreId.HasValue)
            {
                storeIds.Add(customer.RegisteredInStoreId.Value);
            }

            NotifyRegisteredWebHooks(customer, webhookEvent, storeIds);
        }

        public void HandleEvent(EntityInserted<Product> eventMessage)
        {
            ProductDto productDto = _dtoHelper.PrepareProductDTO(eventMessage.Entity);

            // The Store mappings of the product are still not saved, so all webhooks will be triggered
            // no matter for which store are registered.
            NotifyRegisteredWebHooks(productDto, WebHookNames.ProductsCreate, productDto.StoreIds);
        }

        public void HandleEvent(EntityUpdated<Product> eventMessage)
        {
            ProductDto productDto = _dtoHelper.PrepareProductDTO(eventMessage.Entity);

            ProductUpdated(productDto);
        }

        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
            CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(eventMessage.Entity);

            // The Store mappings of the category are still not saved, so all webhooks will be triggered
            // no matter for which store are registered.
            NotifyRegisteredWebHooks(categoryDto, WebHookNames.CategoriesCreate, categoryDto.StoreIds);
        }

        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(eventMessage.Entity);

            string webhookEvent = WebHookNames.CategoriesUpdate;

            if (categoryDto.Deleted == true)
            {
                webhookEvent = WebHookNames.CategoriesDelete;
            }

            NotifyRegisteredWebHooks(categoryDto, webhookEvent, categoryDto.StoreIds);
        }

        public void HandleEvent(EntityInserted<Order> eventMessage)
        {
            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(eventMessage.Entity);

            var storeIds = new List<int>();

            if (orderDto.StoreId.HasValue)
            {
                storeIds.Add(orderDto.StoreId.Value);
            }

            NotifyRegisteredWebHooks(orderDto, WebHookNames.OrdersCreate, storeIds);
        }

        public void HandleEvent(EntityUpdated<Order> eventMessage)
        {
            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(eventMessage.Entity);

            string webhookEvent = WebHookNames.OrdersUpdate;

            if (orderDto.Deleted == true)
            {
                webhookEvent = WebHookNames.OrdersDelete;
            }

            var storeIds = new List<int>();

            if (orderDto.StoreId.HasValue)
            {
                storeIds.Add(orderDto.StoreId.Value);
            }

            NotifyRegisteredWebHooks(orderDto, webhookEvent, storeIds);
        }

        public void HandleEvent(EntityInserted<StoreMapping> eventMessage)
        {
            HandleStoreMappingEvent(eventMessage.Entity.EntityId, eventMessage.Entity.EntityName);
        }

        public void HandleEvent(EntityDeleted<StoreMapping> eventMessage)
        {
            HandleStoreMappingEvent(eventMessage.Entity.EntityId, eventMessage.Entity.EntityName);
        }

        public void HandleEvent(EntityInserted<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == SystemCustomerAttributeNames.FirstName ||
                eventMessage.Entity.Key == SystemCustomerAttributeNames.LastName ||
                eventMessage.Entity.Key == SystemCustomerAttributeNames.LanguageId)
            {
                var customerDto = _customerApiService.GetCustomerById(eventMessage.Entity.EntityId);

                var storeIds = new List<int>();

                if (customerDto.RegisteredInStoreId.HasValue)
                {
                    storeIds.Add(customerDto.RegisteredInStoreId.Value);
                }

                NotifyRegisteredWebHooks(customerDto, WebHookNames.CustomersUpdate, storeIds);
            }
        }

        public void HandleEvent(EntityUpdated<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == SystemCustomerAttributeNames.FirstName ||
                eventMessage.Entity.Key == SystemCustomerAttributeNames.LastName ||
                eventMessage.Entity.Key == SystemCustomerAttributeNames.LanguageId)
            {
                var customerDto = _customerApiService.GetCustomerById(eventMessage.Entity.EntityId);

                var storeIds = new List<int>();

                if (customerDto.RegisteredInStoreId.HasValue)
                {
                    storeIds.Add(customerDto.RegisteredInStoreId.Value);
                }

                NotifyRegisteredWebHooks(customerDto, WebHookNames.CustomersUpdate, storeIds);
            }
        }

        public void HandleEvent(EntityUpdated<Store> eventMessage)
        {
            StoreDto storeDto = _dtoHelper.PrepareStoreDTO(eventMessage.Entity);

            int storeId;

            if (int.TryParse(storeDto.Id, out storeId))
            {
                var storeIds = new List<int>();
                storeIds.Add(storeId);

                NotifyRegisteredWebHooks(storeDto, WebHookNames.StoresUpdate, storeIds);
            }
        }

        public void HandleEvent(EntityInserted<ProductCategory> eventMessage)
        {
            NotifyProductCategoryMappingWebhook(eventMessage.Entity, WebHookNames.ProductCategoryMapsCreate);
        }

        public void HandleEvent(EntityUpdated<ProductCategory> eventMessage)
        {
            NotifyProductCategoryMappingWebhook(eventMessage.Entity, WebHookNames.ProductCategoryMapsUpdate);
        }

        public void HandleEvent(EntityDeleted<ProductCategory> eventMessage)
        {
            NotifyProductCategoryMappingWebhook(eventMessage.Entity, WebHookNames.ProductCategoryMapsDelete);
        }

        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            LanguageDto langaDto = _dtoHelper.PrepateLanguageDto(eventMessage.Entity);

            NotifyRegisteredWebHooks(langaDto, WebHookNames.LanguagesCreate, langaDto.StoreIds);
        }

        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            LanguageDto langaDto = _dtoHelper.PrepateLanguageDto(eventMessage.Entity);

            NotifyRegisteredWebHooks(langaDto, WebHookNames.LanguagesUpdate, langaDto.StoreIds);
        }

        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            LanguageDto langaDto = _dtoHelper.PrepateLanguageDto(eventMessage.Entity);

            NotifyRegisteredWebHooks(langaDto, WebHookNames.LanguagesDelete, langaDto.StoreIds);
        }

        public void HandleEvent(EntityInserted<ProductPicture> eventMessage)
        {
            var product = _productApiService.GetProductById(eventMessage.Entity.ProductId);

            if (product != null)
            {
                ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

                ProductUpdated(productDto);
            }
        }

        public void HandleEvent(EntityUpdated<ProductPicture> eventMessage)
        {
            var product = _productApiService.GetProductById(eventMessage.Entity.ProductId);

            if (product != null)
            {
                ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

                ProductUpdated(productDto);
            }
        }

        public void HandleEvent(EntityDeleted<ProductPicture> eventMessage)
        {
            var product = _productApiService.GetProductById(eventMessage.Entity.ProductId);

            if (product != null)
            {
                ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

                ProductUpdated(productDto);
            }
        }

        // We trigger a product updated WebHook when a picture used in a product is updated.
        // This is required, because when the product title is changed, the product is updated first
        // and then the picture urls are chaged. In order for the WebHook consumer to have the latest
        // product picture urls the following code is used.
        public void HandleEvent(EntityUpdated<Picture> eventMessage)
        {
            var productPicture = _productPictureService.GetProductPictureByPictureId(eventMessage.Entity.Id);

            if (productPicture != null)
            {
                var product = _productApiService.GetProductById(productPicture.ProductId);

                if (product != null)
                {
                    ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

                    ProductUpdated(productDto);
                }
            }
        }

        public void HandleEvent(EntityDeleted<NewsLetterSubscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(Configurations.NEWSLETTER_SUBSCRIBERS_KEY);

            NewsLetterSubscriptionDto newsLetterSubscriptionDto = eventMessage.Entity.ToDto();

            var storeIds = new List<int>
            {
                newsLetterSubscriptionDto.StoreId
            };

            NotifyRegisteredWebHooks(newsLetterSubscriptionDto, WebHookNames.NewsLetterSubscriptionDelete, storeIds);
        }

        public void HandleEvent(EntityInserted<NewsLetterSubscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(Configurations.NEWSLETTER_SUBSCRIBERS_KEY);

            NewsLetterSubscriptionDto newsLetterSubscriptionDto = eventMessage.Entity.ToDto();

            var storeIds = new List<int>
            {
                newsLetterSubscriptionDto.StoreId
            };

            NotifyRegisteredWebHooks(newsLetterSubscriptionDto, WebHookNames.NewsLetterSubscriptionCreate, storeIds);
        }

        public void HandleEvent(EntityUpdated<NewsLetterSubscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(Configurations.NEWSLETTER_SUBSCRIBERS_KEY);

            NewsLetterSubscriptionDto newsLetterSubscriptionDto = eventMessage.Entity.ToDto();

            var storeIds = new List<int>
            {
                newsLetterSubscriptionDto.StoreId
            };

            NotifyRegisteredWebHooks(newsLetterSubscriptionDto, WebHookNames.NewsLetterSubscriptionUpdate, storeIds);
        }

        private void NotifyProductCategoryMappingWebhook(ProductCategory productCategory, string eventName)
        {
            var storeIds = GetStoreIdsForProductCategoryMap(productCategory);

            if (storeIds == null)
            {
                return;
            }

            ProductCategoryMappingDto productCategoryMappingDto = productCategory.ToDto();

            NotifyRegisteredWebHooks(productCategoryMappingDto, eventName, storeIds);
        }

        private List<int> GetStoreIdsForProductCategoryMap(ProductCategory productCategory)
        {
            // Send a webhook event only for the stores that can access the product and category
            // in the current product category map.
            Product product = _productService.GetProductById(productCategory.ProductId);
            Category category = _categoryService.GetCategoryById(productCategory.CategoryId);

            if (product == null || category == null)
            {
                return null;
            }

            var productStoreIds = _storeMappingService.GetStoresIdsWithAccess(product);

            var categoryStoreIds = _storeMappingService.GetStoresIdsWithAccess(category);

            return productStoreIds.Intersect(categoryStoreIds).ToList();
        }

        private void ProductUpdated(ProductDto productDto)
        {
            string webhookEvent = WebHookNames.ProductsUpdate;

            if (productDto.Deleted == true)
            {
                webhookEvent = WebHookNames.ProductsDelete;
            }

            NotifyRegisteredWebHooks(productDto, webhookEvent, productDto.StoreIds);
        }

        private void HandleStoreMappingEvent(int entityId, string entityName)
        {
            // When creating or editing a category after saving the store mapping the category is not updated
            // so we should listen for StoreMapping update/delete and fire a webhook with the updated entityDto(with correct storeIds).
            if (entityName == "Category")
            {
                var category = _categoryApiService.GetCategoryById(entityId);

                if (category != null)
                {
                    CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(category);

                    string webhookEvent = WebHookNames.CategoriesUpdate;

                    if (categoryDto.Deleted == true)
                    {
                        webhookEvent = WebHookNames.CategoriesDelete;
                    }

                    NotifyRegisteredWebHooks(categoryDto, webhookEvent, categoryDto.StoreIds);
                }
            }
            else if (entityName == "Product")
            {
                var product = _productApiService.GetProductById(entityId);

                if (product != null)
                {
                    ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

                    string webhookEvent = WebHookNames.ProductsUpdate;

                    if (productDto.Deleted == true)
                    {
                        webhookEvent = WebHookNames.ProductsDelete;
                    }

                    NotifyRegisteredWebHooks(productDto, webhookEvent, productDto.StoreIds);
                }
            }
        }

        private void NotifyRegisteredWebHooks<T>(T entityDto, string webhookEvent, List<int> storeIds)
        {
            if (storeIds.Count > 0)
            {
                // Notify all webhooks that the entity is mapped to their store.
                _webHookManager.NotifyAllAsync(webhookEvent, new { Item = entityDto }, (hook, hookUser) => IsEntityMatchingTheWebHookStoreId(hookUser, storeIds));

                if (typeof(T) == typeof(ProductDto) || typeof(T) == typeof(CategoryDto))
                {
                    NotifyUnmappedEntityWebhooks(entityDto, storeIds);
                }
            }
            else
            {
                _webHookManager.NotifyAllAsync(webhookEvent, new { Item = entityDto });
            }
        }

        private void NotifyUnmappedEntityWebhooks<T>(T entityDto, List<int> storeIds)
        {
            if (typeof(T) == typeof(ProductDto))
            {
                // The product is not mapped to the store.
                // Notify all webhooks that the entity is not mapped to their store.
                _webHookManager.NotifyAllAsync(WebHookNames.ProductsUnmap, new { Item = entityDto },
                    (hook, hookUser) => !IsEntityMatchingTheWebHookStoreId(hookUser, storeIds));
            }
            else if (typeof(T) == typeof(CategoryDto))
            {
                // The category is not mapped to the store.
                // Notify all webhooks that the entity is not mapped to their store.
                _webHookManager.NotifyAllAsync(WebHookNames.CategoriesUnmap, new { Item = entityDto },
                    (hook, hookUser) => !IsEntityMatchingTheWebHookStoreId(hookUser, storeIds));
            }
        }

        private bool IsEntityMatchingTheWebHookStoreId(string webHookUser, List<int> storeIds)
        {
            // When we register the webhooks we add "-storeId" at the end of the webHookUser string.
            // That way we can check to which store is mapped the webHook.
            foreach (var id in storeIds)
            {
                if (webHookUser.EndsWith("-" + id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}