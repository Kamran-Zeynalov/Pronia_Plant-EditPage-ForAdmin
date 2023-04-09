namespace P230_Pronia.Entities
{
    public class Size:BaseEntity
    {
        public string Name { get; set; } = null!;
        public List<PlantSizeColor> PlantSizeColors { get; set; }
        public Size()
        {
            PlantSizeColors = new();

        }
    }
}
