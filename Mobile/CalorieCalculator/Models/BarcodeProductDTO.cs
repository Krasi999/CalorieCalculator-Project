namespace CalorieCalculator.Models;

public class BarcodeProductDTO
{
    public bool Found { get; set; }

    public bool FromLocal { get; set; }

    public int? ProductID { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    public int Calories { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbs { get; set; }

    public decimal Fats { get; set; }

    public int Weight { get; set; }

    public int CategoryID { get; set; }

    public string Barcode { get; set; }
}
