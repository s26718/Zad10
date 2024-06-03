using System.ComponentModel.DataAnnotations;

namespace Zad10.Models;

public class Doctor
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    [MaxLength(100)]
    public string Email { get; set; }
    public virtual ICollection<Prescription> Prescriptions { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }
}