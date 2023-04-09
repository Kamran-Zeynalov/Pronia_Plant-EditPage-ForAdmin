using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
using P230_Pronia.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P230_Pronia.ViewModels
{
    public class PlantVM
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string SKU { get; set; }
        public string Desc { get; set; }
        public int PlantDeliveryInformationId { get; set; }
        [NotMapped]
        public ICollection<int> CategoryIds { get; set; } = null!;
        [NotMapped]
        public ICollection<int> TagIds { get; set; } = null!;
        [NotMapped]
        public IFormFile? MainPhoto { get; set; }
        [NotMapped]
        public IFormFile? HoverPhoto { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? Images { get; set; }
        public ICollection<PlantImage>? SpecificImages { get; set; }
        public ICollection<int>? ImageIds { get; set; }
        public string? ColorSizeQuantity { get; set; }
        public ICollection<PlantSizeColor>? PlantSizeColors { get; set; }
        public string? PlantSizeColorsId { get; set; }

        public PlantVM()
        {
            PlantSizeColors = new List<PlantSizeColor>();
        }



    }
}
