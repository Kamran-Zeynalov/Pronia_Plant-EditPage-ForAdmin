namespace P230_Pronia.Entities
{
    public class PlantTag:BaseEntity
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public Plant Plant { get; set; }
    }
}
