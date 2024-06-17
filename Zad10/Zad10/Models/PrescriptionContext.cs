using Microsoft.EntityFrameworkCore;
namespace Zad10.Models;

public class PrescriptionContext : DbContext
{
    
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public PrescriptionContext() 
    {
    }

    public PrescriptionContext(DbContextOptions<PrescriptionContext> options) : base(options)
    {
    }
    
}