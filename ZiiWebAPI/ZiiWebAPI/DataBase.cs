using Microsoft.EntityFrameworkCore;
using ClassDataBaseLibrary;

namespace ZiiWebAPI
{
    public class DataBase : DbContext
    {
        private string dataBaseName = string.Empty;

        private DbSet<User> User { get; set; } = null;
        private DbSet<Country> Country { get; set; } = null;
        private DbSet<University> University { get; set; } = null;
        private DbSet<Faculty> Faculty { get; set; } = null;
        private DbSet<Programa> Program { get; set; } = null;
        private DbSet<Student> Student { get; set; } = null;
        private DbSet<Application> Application { get; set; } = null;
        private DbSet<ApplicationStatus> ApplicationStatus { get; set; } = null;
        private DbSet<ApplicationStatusHistory> ApplicationStatusHistory { get; set; } = null;


        public DataBase(string dataBaseName)
        {
            this.dataBaseName = dataBaseName;
            Database.EnsureCreated();
        }

        //создание таблиц в бд 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasData(
            //        new User { Id = 1, Login = "admin", Password = "admin" }
            //    );

            modelBuilder.Entity<ApplicationStatus>().HasData(
                new ApplicationStatus { Id = 1, StatusName = "Отправлен" },
                new ApplicationStatus { Id = 2, StatusName = "Принят" },
                new ApplicationStatus { Id = 3, StatusName = "Зачислен" }
            );

            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, CountryName = "Angola" },
                new Country { Id = 2, CountryName = "China" },
                new Country { Id = 3, CountryName = "Russia" },
                new Country { Id = 4, CountryName = "China" },
                new Country { Id = 5, CountryName = "Brazil" },
                new Country { Id = 6, CountryName = "India" },
                new Country { Id = 7, CountryName = "Ghana" },
                new Country { Id = 8, CountryName = "Belarus" },
                new Country { Id = 9, CountryName = "China" },
                new Country { Id = 10, CountryName = "Nigeria" },
                new Country { Id = 11, CountryName = "Ghana" },
                new Country { Id = 12, CountryName = "Egypt" },
                new Country { Id = 13, CountryName = "USA" },
                new Country { Id = 14, CountryName = "Congo" },
                new Country { Id = 15, CountryName = "Britain" },
                new Country { Id = 16, CountryName = "Japan" },
                new Country { Id = 17, CountryName = "Ireland" },
                new Country { Id = 18, CountryName = "Finland" },
                new Country { Id = 19, CountryName = "Hungary" },
                new Country { Id = 20, CountryName = "Rwanda" },
                new Country { Id = 21, CountryName = "Qatar" },
                new Country { Id = 22, CountryName = "Denmark" },
                new Country { Id = 23, CountryName = "Sweden" },
                new Country { Id = 24, CountryName = "Canada" },
                new Country { Id = 25, CountryName = "Somali" }
            );

            modelBuilder.Entity<University>().HasData(
                new University { Id = 1, UniversityName = "National University of Science and Technology MISiS" },
                new University { Id = 2, UniversityName = "National Research University of Electronic Technology" },
                new University { Id = 4, UniversityName = "Moscow Institute of Physics and Technology" },
                new University { Id = 5, UniversityName = "Volgograd state technical university" },
                new University { Id = 6, UniversityName = "Belgorod Shukhov state technological university" },
                new University { Id = 7, UniversityName = "Chelyabinsk State University" },
                new University { Id = 8, UniversityName = "Irkutsk State University" },
                new University { Id = 9, UniversityName = "Kaliningrad State Technical University" },
                new University { Id = 10, UniversityName = "Kazan Federal University" },
                new University { Id = 11, UniversityName = "Kazan State Medical University" },
                new University { Id = 12, UniversityName = "Moscow Aviation Institute " },
                new University { Id = 13, UniversityName = "Murmansk State Technical University" },
                new University { Id = 14, UniversityName = "VGSPU" }
            );

            modelBuilder.Entity<Faculty>().HasData(
                new Faculty { Id = 1, FacultyName = "Social sciences", UniversityId = 1 },
                new Faculty { Id = 2, FacultyName = "Arts", UniversityId = 1 },
                new Faculty { Id = 3, FacultyName = "Science", UniversityId = 1 },
                new Faculty { Id = 4, FacultyName = "History", UniversityId = 1 },
                new Faculty { Id = 5, FacultyName = "English", UniversityId = 1 }

            );

            modelBuilder.Entity<Programa>().HasData(
                new Programa { Id = 1, ProgramName = "Бакалавр", FacultyId = 1 },
                new Programa { Id = 2, ProgramName = "Магистратура>", FacultyId = 1 },
                new Programa { Id = 3, ProgramName = "Аспиратнута", FacultyId = 1 },
                new Programa { Id = 4, ProgramName = "Бакалавр", FacultyId = 2 },
                new Programa { Id = 5, ProgramName = "Магистратура>", FacultyId = 2 },
                new Programa { Id = 6, ProgramName = "Бакалавр", FacultyId = 3 },
                new Programa { Id = 7, ProgramName = "Магистратура>", FacultyId = 3 },
                new Programa { Id = 8, ProgramName = "Аспиратнута>", FacultyId = 3 },
                new Programa { Id = 9, ProgramName = "Бакалавр", FacultyId = 4 },
                new Programa { Id = 10, ProgramName = "Бакалавр", FacultyId = 5 }
            );

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={dataBaseName}.db");
        }
    }
}
