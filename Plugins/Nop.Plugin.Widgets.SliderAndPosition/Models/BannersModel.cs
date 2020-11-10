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

        [NopResourceDisplayName("plugins.bannerandslider.banner.fields.displayorder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("plugins.bannerandslider.banner.fields.title")]
        public string Title { get; set; }

        [NopResourceDisplayName("plugins.bannerandslider.banner.fields.description")]
        public string Description { get; set; }

        [NopResourceDisplayName("plugins.bannerandslider.banner.fields.url")]
        public string URL { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("plugins.bannerandslider.banner.fields.pictureurl")]
        public string PictureUrl { get; set; }
    }
}