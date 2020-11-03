using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.SliderAndPosition
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.SliderAndPosition.Configure",
                 "Plugins/SliderAndPosition/Configure",
                 new { controller = "SliderAndPosition", action = "Configure", },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.SliderAndPosition.Create",
                 "Plugins/SliderAndPosition/Create",
                 new { controller = "SliderAndPosition", action = "Create", },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.SliderAndPosition.Edit",
                 "Plugins/SliderAndPosition/Edit",
                 new { controller = "SliderAndPosition", action = "Edit" },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.SliderAndPosition.Delete",
                 "Plugins/SliderAndPosition/Delete",
                 new { controller = "SliderAndPosition", action = "Delete" },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.SliderAndPosition.ListBannerBy",
                 "Plugins/SliderAndPosition/ListBannerByPositionId",
                 new { controller = "SliderAndPosition", action = "ListBannerByPositionId" },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.SliderAndPosition.PictureUpdate",
                 "Plugins/SliderAndPosition/PictureUpdate",
                 new { controller = "SliderAndPosition", action = "PictureUpdate" },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.SliderAndPosition.PictureDelete",
                 "Plugins/SliderAndPosition/PictureDelete",
                 new { controller = "SliderAndPosition", action = "PictureDelete" },
                 new[] { "Nop.Plugin.Widgets.SliderAndPosition.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
