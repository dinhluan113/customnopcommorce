using Nop.Data.Mapping;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;

namespace Nop.Plugin.Widgets.SliderAndPosition.Data
{
    public partial class SliderAndPositionMap : NopEntityTypeConfiguration<SliderAndPositionMd>
    {
        public SliderAndPositionMap()
        {
            this.ToTable("SliderAndPosition");
            this.HasKey(point => point.Id);
            //Map the additional properties
            this.Property(record => record.Name);
        }
    }
}