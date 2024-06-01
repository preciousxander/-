using ClassDataBaseLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace XanderFront.Pages
{
    public class ShowApplication
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public string ProgramName { get; set; }
        public string FacultyName { get; set; }
        public DateTime Date { get; set; }
        public string Status {  get; set; }
    }

    public class ApplicationStatusModel : PageModel
    {
        public List<ShowApplication> showApplications = new List<ShowApplication>();

        public void OnGet()
        {
            RestClient client = new RestClient("https://localhost:7227");

            var dataApp = client.Get<Application>("api/Application");

            if (dataApp != null)
            {
                int count = 1;
                var statusApp = client.Get<ApplicationStatusHistory>("api/ApplicationStatusHistory");
                var stats = client.Get<ApplicationStatus>("api/ApplicationStatus");

                foreach (var item in dataApp)
                {
                    var stud = client.Get<Student>("api/Student", item.StudentId);
                    var stat = statusApp.FirstOrDefault(statusApp => statusApp.ApplicationId == item.Id);

                    var prog = client.Get<Programa>("api/Programa", item.ProgramId);
                    var fac = client.Get<Faculty>("api/Faculty", prog.FacultyId).FacultyName;

                    showApplications.Add(new ShowApplication()
                    {
                        Count = count,
                        Name = stud.FirstName + " " + stud.LastName,
                        ProgramName = prog.ProgramName,
                        FacultyName = fac,
                        Date = stat.StatusDate,
                        Status = stats.FirstOrDefault(st => st.Id == stat.ApplicationStatusId).StatusName
                    });

                    count++;
                }
            }
        }
    }
}
