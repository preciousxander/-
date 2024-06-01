using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace ClassDataBaseLibrary
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }


    [Index(nameof(Login), IsUnique = true)]
    public class User : BaseModel
    {
        
        [Required]
        public string Login { get; set; }
        private string _password;

        [Required]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SHA256 sha256 = SHA256.Create();
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                _password = BitConverter.ToString(hashValue).Replace("-", "");
            }
        }
    }

    public class Country : BaseModel
    {
        public string CountryName { get; set; }
    }

    public class University : BaseModel
    {
        public string UniversityName { get; set; }
    }

    public class Faculty : BaseModel
    {
        public string FacultyName { get; set; }
        public int UniversityId { get; set; }

        [JsonIgnore]
        public University? University { get; set; }
    }

    public class Programa : BaseModel
    {
        public string ProgramName { get; set; }
        public int FacultyId { get; set; }

        [JsonIgnore]
        public Faculty? Faculty { get; set; }
    }

    public class Student : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Sex { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birth { get; set; }
        public int UserId { get; set; }
        public int CountryId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Country? Country { get; set; }
    }

    public class Application : BaseModel
    {
        public int StudentId { get; set; }
        public int ProgramId { get; set; }
        public DateTime ApplicationDate { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }
        [JsonIgnore]
        public Programa? Program { get; set; }
    }

    public class ApplicationStatus : BaseModel
    {
        public string StatusName { get; set; }
    }

    public class ApplicationStatusHistory : BaseModel
    {
        public int ApplicationId { get; set; }
        public int ApplicationStatusId { get; set; }
        public DateTime StatusDate { get; set; }

        [JsonIgnore]
        public Application? Application { get; set; }
        [JsonIgnore]
        public ApplicationStatus? ApplicationStatus { get; set; }
    }

}
