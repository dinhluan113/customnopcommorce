using System;
using System.Collections.Generic;
using System.Linq;
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
            IStoreService storeService,
            IPictureService pictureService)
        {
            this._addressService = addressService;
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._stateProvinceService = stateProvinceService;
            this._sliderAndPositionService = sliderAndPositionService;
            this._storeService = storeService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
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
                Description = model.Description,
            };

            var pic1url = _pictureService.GetPictureUrl(model.image_1_id);
            dtoMd.image_1_id = model.image_1_id;
            dtoMd.image_1_src = pic1url;

            _sliderAndPositionService.InsertSliderAndPosition(dtoMd);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View("~/Plugins/Nop.Plugin.Widgets.SliderAndPosition/Views/Create.cshtml", model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sliderAndPositionModel = _sliderAndPositionService.GetSliderAndPositionById(id);
            if (sliderAndPositionModel == null)
                return RedirectToAction("Configure");

            //var pic1 = _pictureService.GetPictureById(sliderAndPositionModel.image1);
            var model = new SliderAndPositionModel
            {
                Id = sliderAndPositionModel.Id,
                Name = sliderAndPositionModel.Name,
                Description = sliderAndPositionModel.Description,
                image_1_id = sliderAndPositionModel.image_1_id,
                image_1_src = sliderAndPositionModel.image_1_src,
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
                return RedirectToAction("Configure");

            var pic1url = _pictureService.GetPictureUrl(model.image_1_id);
            sliderAndPositionModel.image_1_id = model.image_1_id;
            sliderAndPositionModel.image_1_src = pic1url;

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sliderAndPositionModel = _sliderAndPositionService.GetSliderAndPositionById(id);
            if (sliderAndPositionModel == null)
                return RedirectToAction("Configure");

            _sliderAndPositionService.DeleteSliderAndPosition(sliderAndPositionModel);

            return new NullJsonResult();
        }

        #endregion
    }
}
