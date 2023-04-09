namespace P230_Pronia.Entities
{
    public class PlantImage:BaseEntity
    {
        public string Path { get; set; }
        public bool? IsMain { get; set; }
        public Plant Plant { get; set; }
    }
}
