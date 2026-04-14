using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.FoodDTOs;

[Table("FoodCategories")]
public class FoodCategory
{
    [Key]
    public int CategoryID { get; set; }

    public string? CategoryName { get; set; }

    public ICollection<FoodProduct> FoodProducts { get; set; } = new List<FoodProduct>();
}
