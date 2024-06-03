namespace Zad10.Repositories;

using System;
using System.Linq;
using System.Threading.Tasks;
using Zad10.Models;

public static class DbInitializer
{
    public static async Task Initialize(PrescriptionContext context)
    {
        if (context.Patients.Any() || context.Doctors.Any() || context.Medicaments.Any())
        {
            return; // DB has been seeded
        }

        var patients = new Patient[]
        {
            new Patient { FirstName = "John", LastName = "Doe", Birthdate = new DateTime(1980, 1, 1) },
            new Patient { FirstName = "Jane", LastName = "Doe", Birthdate = new DateTime(1990, 1, 1) }
        };
        foreach (var p in patients)
        {
            context.Patients.Add(p);
        }
        await context.SaveChangesAsync();

        var doctors = new Doctor[]
        {
            new Doctor { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" },
            new Doctor { FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com" }
        };
        foreach (var d in doctors)
        {
            context.Doctors.Add(d);
        }
        await context.SaveChangesAsync();

        var medicaments = new Medicament[]
        {
            new Medicament { Name = "Aspirin", Description = "Pain reliever", Type = "Tablet" },
            new Medicament { Name = "Penicillin", Description = "Antibiotic", Type = "Injection" }
        };
        foreach (var m in medicaments)
        {
            context.Medicaments.Add(m);
        }
        await context.SaveChangesAsync();
    }
}
