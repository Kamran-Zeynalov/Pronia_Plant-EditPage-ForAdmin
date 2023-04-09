namespace P230_Pronia.Entities
{
    public class PlantSizeColor:BaseEntity
    {
        public int PlantId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public Plant Plant { get; set; } = null!;
        public Color Color { get; set; } = null!;
        public Size Size { get; set; } = null!;
    }
}
