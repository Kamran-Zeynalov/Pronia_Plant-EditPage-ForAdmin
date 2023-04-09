namespace P230_Pronia.Entities
{
    public class Tag:BaseEntity
    {
        public string Name { get; set; }

        public List<PlantTag> PlantTags { get; set; }
    }
}
