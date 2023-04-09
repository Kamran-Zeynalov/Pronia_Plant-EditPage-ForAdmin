namespace P230_Pronia.Entities
{
    public class PlantDeliveryInformation:BaseEntity
    {
        public string Shipping { get; set; }
        public string AboutReturnRequest { get; set; }
        public string Guarantee{ get; set; }
        public List<Plant> Plants { get; set; }
    }
}
