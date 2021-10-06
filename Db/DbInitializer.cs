using csharp_lend_pc.Models;
using System;
using System.Linq;

namespace csharp_lend_pc.Db
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.employees.Any())
            {
                return;   // DB has been seeded
            }

            var employees = new EmployeeEntity[]
            {
            new EmployeeEntity{Id="B10003",FirstName="a",LastName="Alexander", FirstNameKana="a", LastNameKana="a", Position="a", Department="a", Privilege="a", Gender="a", Age=1, TelNumber="a", Email="a" },
            };
            foreach (EmployeeEntity e in employees)
            {
                context.employees.Add(e);
            }
            context.SaveChanges();

            context.Database.EnsureCreated();

            // Look for any students.
            if (context.pcs.Any())
            {
                return;   // DB has been seeded
            }

            var pcs = new PcEntity[]
            {
            new PcEntity{Id="B10003",Maker="a",Os="Alexander", Memory="a", Capacity="a", StorageLocation="a", Remarks="a", HasGraphicBoard=false, IsBroken=false, IsDeleted=false, LeaseStartAt=DateTime.Now, LeaseEndAt=DateTime.Now },
            };
            foreach (PcEntity p in pcs)
            {
                context.pcs.Add(p);
            }
            context.SaveChanges();
        }
    }
}