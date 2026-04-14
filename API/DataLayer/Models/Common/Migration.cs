using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Common;

[Table("SystemMigrations")]
public class Migration
{
    [Key]
    public Guid MigrationID { get; set; }

    public string? FileName { get; set; }

    public DateTime? CreatedAt { get; set; }
}
