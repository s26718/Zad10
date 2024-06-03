using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Zad10.Models;
[PrimaryKey(nameof(IdMedicament), nameof(IdPrescription))]
public class PrescriptionMedicament
{
    public int IdMedicament { get; set; }
    public int IdPrescription { get; set; }
    public int Dose { get; set; }
    [MaxLength(100)]
    public string? Details { get; set; }
    [ForeignKey("IdMedicament")]
    public virtual Medicament Medicament { get; set; }
    [ForeignKey("IdPrescription")]
    public virtual Prescription Prescription { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }
}