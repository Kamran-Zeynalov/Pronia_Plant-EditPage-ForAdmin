using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P230_Pronia.Entities
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage="Zehmet olmasa inputu doldurun")]
        [StringLength(maximumLength:20,ErrorMessage ="20-den uzun xarakter daxil oluna bilmez")]
        public string Name { get; set; }
        public List<PlantCategory> PlantCategories{ get; set; }
        public Category()
        {
            PlantCategories = new();
        }
    }
}
