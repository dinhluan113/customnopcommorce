using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Widgets.SliderAndPosition.Domain
{
    public class SliderAndPositionMd : BaseEntity
    {
        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public int image_1_id { get; set; }
        public string image_1_src { get; set; }
        public int image_2_id { get; set; }
        public string image_2_src { get; set; }
        public int image_3_id { get; set; }
        public string image_3_src { get; set; }
        public int image_4_id { get; set; }
        public string image_4_src { get; set; }
        public int image_5_id { get; set; }
        public string image_5_src { get; set; }
        public int image_6_id { get; set; }
        public string image_6_src { get; set; }
        public int image_7_id { get; set; }
        public string image_7_src { get; set; }
        public int image_8_id { get; set; }
        public string image_8_src { get; set; }
        public int image_9_id { get; set; }
        public string image_9_src { get; set; }
        public int image_10_id { get; set; }
        public string image_10_src { get; set; }
        public bool IsAutoPLay { get; set; }
    }
}