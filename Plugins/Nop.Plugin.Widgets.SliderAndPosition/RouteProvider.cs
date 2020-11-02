using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.SliderAndPosition
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
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
