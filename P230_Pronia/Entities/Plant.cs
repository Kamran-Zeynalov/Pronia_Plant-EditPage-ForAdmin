using System.ComponentModel.DataAnnotations.Schema;

namespace P230_Pronia.Entities
{
    public class Plant : BaseEntity
    {
        public string Name { get; set; }
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(6,2)")]
        public decimal? DiscountPrice { get; set; }
        public string SKU { get; set; }
        public string Desc { get; set; }
        public int PlantDeliveryInformationId { get; set; }
        public PlantDeliveryInformation PlantDeliveryInformation { get; set; }
        public List<PlantImage> PlantImages { get; set; }
        public List<PlantCategory> PlantCategories { get; set; }
        public List<PlantTag> PlantTags { get; set; }
        public List<PlantSizeColor> PlantSizeColors { get; set; }
        public Plant()
        {
            PlantImages = new();
            PlantCategories = new();
            PlantTags = new();
            PlantSizeColors = new();
        }

    }
}
