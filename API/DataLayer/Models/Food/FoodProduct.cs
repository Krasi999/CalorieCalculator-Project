using DataLayer.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace DataLayer.Models;

[Table("FoodProducts")]
public class FoodProduct
{
    [Key]
    public int ProductID { get; private set; }

    public string? Name { get; private set; }

    public string? Description { get; private set; }

    public int Calories { get; private set; }

    public decimal Fats { get; private set; }

    public decimal Protein { get; private set; }

    public decimal Carbs { get; private set; }

    public int Weight { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public int CategoryID { get; private set; }

    [ForeignKey("CategoryID")]
    public FoodCategory? Category { get; set; }

    public string? Barcode { get; private set; }


    public void Update(
        string name, 
        string description,
        int calories,
        int weight,
        decimal fats, 
        decimal protein, 
        decimal carbs,
        FoodCategories categoryId,
        string barcode)
    {
        Name = name;
        Description = description;
        Calories = calories;
        Weight = weight;
        Fats = fats;
        Protein = protein;
        Carbs = carbs;
        CategoryID = (int)categoryId;
        Barcode = barcode;
    }
}
