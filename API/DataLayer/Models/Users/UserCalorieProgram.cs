using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Users;

[Table("UserCaloriePrograms")]
public class UserCalorieProgram
{
    [Key]
    public int ID { get; private set; }


}