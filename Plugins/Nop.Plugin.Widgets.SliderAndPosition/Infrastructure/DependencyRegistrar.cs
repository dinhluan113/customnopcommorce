using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.SliderAndPosition.Data;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;
using Nop.Plugin.Widgets.SliderAndPosition.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.SliderAndPosition.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<SliderAndPositionService>().As<ISliderAndPositionService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<SliderAndPositionObjectContext>(builder, "nop_object_context_slider_and_position");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<SliderAndPositionMd>>()
                .As<IRepository<SliderAndPositionMd>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_slider_and_position"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
