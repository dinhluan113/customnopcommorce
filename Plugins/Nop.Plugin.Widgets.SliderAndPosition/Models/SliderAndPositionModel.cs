using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.SliderAndPosition.Models
{
    public class SliderAndPositionModel : BaseNopEntityModel
    {
        public SliderAndPositionModel()
        {
            AddPictureModel = new BannersModel();
        }
        public string Name { get; set; }

        public BannersModel AddPictureModel { get; set; }
        public IList<BannersModel> LstBanners { get; set; }
    }
}