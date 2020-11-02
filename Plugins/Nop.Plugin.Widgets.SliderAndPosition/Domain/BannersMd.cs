using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Widgets.SliderAndPosition.Domain
{
    public class BannersMd : BaseEntity
    {
        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public int PictureId { get; set; }
        public string PictureSrc { get; set; }
        public int PositionId { get; set; }
        public int DisplayOrder { get; set; }
    }
}