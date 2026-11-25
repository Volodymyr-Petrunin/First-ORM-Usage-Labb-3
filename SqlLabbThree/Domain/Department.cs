using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SqlLabbThree.Domain;

[Table("Departments")]
[Index(nameof(Name), IsUnique = true)]
public class Department
{
    [Key]
    [Column("department_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("department_name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    public List<Employee> Employees { get; set; } = new List<Employee>();
}