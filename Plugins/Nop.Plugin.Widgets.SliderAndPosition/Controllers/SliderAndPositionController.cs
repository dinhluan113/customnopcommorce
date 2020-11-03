using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;
using Nop.Plugin.Widgets.SliderAndPosition.Models;
using Nop.Plugin.Widgets.SliderAndPosition.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Widgets.SliderAndPosition.Controllers
{
    [AdminAuthorize]
    public class SliderAndPositionController : BasePluginController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ISliderAndPositionService _sliderAndPositionService;
        private readonly ILDBannerService _lDBannerService;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public SliderAndPositionController(IAddressService addressService,
            ICountryService countryService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStateProvinceService stateProvinceService,
            ISliderAndPositionService sliderAndPositionService,
            ILDBannerService lDBannerService,
            IStoreService storeService,
            IPictureService pictureService)
        {
            this._addressService = addressService;
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._stateProvinceService = stateProvinceService;
            this._sliderAndPositionService = sliderAndPositionService;
            this._lDBannerService = lDBannerService;
            this._storeService = storeService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Methods

        //[ChildActionOnly]
        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            return View("~/Plugins/Nop.Plugin.Widgets.SliderAndPosition/Views/Configure.cshtml");
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return ErrorForKendoGridJson("Access denied");

            var pickupPoints = _sliderAndPositionService.GetAllSliderAndPositions();
            var model = pickupPoints.Select(x =>
            {
                return new SliderAndPositionModel
                {
                    Id = x.Id,
                    Name = x.Name
                };
            }).ToList();

            return Json(new DataSourceResult
            {
                Data = model,
                Total = pickupPoints.TotalCount
            });
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult ListBannerByPositionId(DataSourceRequest command, int positionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return ErrorForKendoGridJson("Access denied");

            var pickupPoints = _lDBannerService.GetAllBannerByPositionId(positionId);
            if (pickupPoints != null && pickupPoints.Any())
            {
                var model = pickupPoints.Select(x =>
                {
                    return new BannersModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        URL = x.URL,
                        PictureId = x.PictureId,
                        PictureUrl = x.PictureSrc,
                        DisplayOrder = x.DisplayOrder,
                        Description = x.Description,
                    };
                }).ToList();

                return Json(new DataSourceResult
                {
                    Data = model,
                    Total = pickupPoints.TotalCount
                });
            }

            return Json(new DataSourceResult
            {
                Data = new List<BannersModel>(),
                Total = 0
            });
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var model = new SliderAndPositionModel();

            return View("~/Plugins/Nop.Plugin.Widgets.SliderAndPosition/Views/Create.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Create(string btnId, string formId, SliderAndPositionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var dtoMd = new SliderAndPositionMd
            {
                Name = model.Name,
            };

            _sliderAndPositionService.InsertSliderAndPosition(dtoMd);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return RedirectToAction("Configure");
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sliderAndPositionModel = _sliderAndPositionService.GetSliderAndPositionById(id);
            if (sliderAndPositionModel == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var model = new SliderAndPositionModel
            {
                Id = sliderAndPositionModel.Id,
                Name = sliderAndPositionModel.Name
            };

            return View("~/Plugins/Nop.Plugin.Widgets.SliderAndPosition/Views/Edit.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Edit(string btnId, string formId, SliderAndPositionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sliderAndPositionModel = _sliderAndPositionService.GetSliderAndPositionById(model.Id);
            if (sliderAndPositionModel == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            sliderAndPositionModel.Name = model.Name;
            _sliderAndPositionService.UpdateSliderAndPosition(sliderAndPositionModel);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View("~/Plugins/Nop.Plugin.Widgets.SliderAndPosition/Views/Edit.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings) || id == 1 || id == 2 || id == 3)
                return Content("Access denied");

            var sliderAndPositionModel = _sliderAndPositionService.GetSliderAndPositionById(id);
            if (sliderAndPositionModel == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            _sliderAndPositionService.DeleteSliderAndPosition(sliderAndPositionModel);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProductPictureAdd(string btnId, string formId, BannersModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var dtoMd = new BannersMd
            {
                Title = model.Title,
                URL = model.URL,
                PictureId = model.PictureId,
                PictureSrc = _pictureService.GetPictureUrl(model.PictureId),
                PositionId = model.PositionId,
                DisplayOrder = model.DisplayOrder,
                Description = model.Description,
            };

            _lDBannerService.InsertLDBanner(dtoMd);

            ViewBag.RefreshPage = false;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PictureUpdate(string btnId, string formId, BannersModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var bannerDto = _lDBannerService.GetBannerById(model.Id);

            bannerDto.Title = model.Title;
            bannerDto.URL = model.URL;
            bannerDto.DisplayOrder = model.DisplayOrder;
            bannerDto.Description = model.Description;
            if (bannerDto.PictureId != model.PictureId)
            {
                bannerDto.PictureId = model.PictureId;
                bannerDto.PictureSrc = _pictureService.GetPictureUrl(model.PictureId);
            }

            _lDBannerService.UpdateLDBanner(bannerDto);

            ViewBag.RefreshPage = false;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PictureDelete(string btnId, string formId, BannersModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var bannerDto = _lDBannerService.GetBannerById(model.Id);
            _lDBannerService.DeleteLDBanner(bannerDto);

            ViewBag.RefreshPage = false;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
