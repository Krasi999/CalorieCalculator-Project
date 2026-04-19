using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models;

[Table("CaloriePrograms")]
public class CalorieProgram
{
    public CalorieProgram (int caloriesPerDay, int carbsPerDay, int proteinPerDay, int fatsPerDay, DateTime programDate, Guid userID)
    {
        CaloriesPerDay = caloriesPerDay;
        CarbsPerDay = carbsPerDay;
        ProteinPerDay = proteinPerDay;
        FatsPerDay = fatsPerDay;
        ProgramDate = programDate;
        UserID = userID;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProgramID { get; private set; }

    public int CaloriesPerDay { get; private set; }

    public int CarbsPerDay { get; private set; }

    public int ProteinPerDay { get; private set; }

    public int FatsPerDay { get; private set; }

    public DateTime ProgramDate { get; private set; }

    public Guid UserID { get; private set; }

    [ForeignKey("UserID")]
    public User? User { get; set; }

    public ICollection<CalorieProgramMeal> CalorieProgramMeals { get; set; } = new List<CalorieProgramMeal>();
}
