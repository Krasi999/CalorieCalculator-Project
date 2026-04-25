public class FoodProductRequest
{
    public int? ProductID { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public int Calories { get; set; }

    public int Weight { get; set; }

    public decimal Fats { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbs { get; set; }

    public int CategoryID { get; set; }

    public string? Barcode { get; set; }
}