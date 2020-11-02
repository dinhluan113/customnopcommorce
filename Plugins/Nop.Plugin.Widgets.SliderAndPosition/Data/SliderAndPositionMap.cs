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
            //this.Property(record => record.IsAutoPLay);
            //this.Property(record => record.image1);
            //this.Property(record => record.image2);
            //this.Property(record => record.image3);
            //this.Property(record => record.image4);
            //this.Property(record => record.image5);
            //this.Property(record => record.image6);
            //this.Property(record => record.image7);
            //this.Property(record => record.image8);
            //this.Property(record => record.image9);
            //this.Property(record => record.image10);
            //this.Property(record => record.URL).HasMaxLength(255);
        }
    }
}