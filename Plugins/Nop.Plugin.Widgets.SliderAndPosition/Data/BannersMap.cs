using Nop.Data.Mapping;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;

namespace Nop.Plugin.Widgets.SliderAndPosition.Data
{
    public partial class BannersMap : NopEntityTypeConfiguration<BannersMd>
    {
        public BannersMap()
        {
            this.ToTable("Banners");
            this.HasKey(point => point.Id);
        }
    }
}