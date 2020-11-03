using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.SliderAndPosition.Models
{
    public class BannersModel : BaseNopEntityModel
    {
        public BannersModel()
        {
            Title = "";
            Description = "";
            URL = "";
        }
        public int PositionId { get; set; }
        public int DisplayOrder { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
    }
}