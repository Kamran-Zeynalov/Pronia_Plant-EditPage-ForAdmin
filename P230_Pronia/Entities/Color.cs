namespace P230_Pronia.Entities
{
    public class Color:BaseEntity
    {
        public string Name { get; set; } = null!;
        public List<PlantSizeColor> PlantSizeColors { get; set; }
        public Color()
        {
            PlantSizeColors = new();
        }
    }
}
