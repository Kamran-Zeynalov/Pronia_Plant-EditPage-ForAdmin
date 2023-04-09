namespace P230_Pronia.Entities
{
    public class PlantCategory:BaseEntity
    {
        public int CategoryId { get; set; } 
        public Plant Plant { get; set; }
        public Category Category { get; set; }

    }

}
