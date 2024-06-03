using Microsoft.EntityFrameworkCore;
namespace Zad10.Models;

public class PrescriptionContext : DbContext
{
    
    public virtual DbSet<Medicament> Medicaments { get; set; }
    public virtual DbSet<Prescription> Prescriptions { get; set; }
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Doctor> Doctors { get; set; }
    public virtual DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public PrescriptionContext() 
    {
    }

    public PrescriptionContext(DbContextOptions<PrescriptionContext> options) : base(options)
    {
    }
    
}