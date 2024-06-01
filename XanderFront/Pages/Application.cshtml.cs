using ClassDataBaseLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace XanderFront.Pages
{
    public class ApplicationModel : PageModel
    {
        private RestClient restClient = new RestClient("https://localhost:7227");

        public List<University> univer;

        public void OnGet()
        {
            var data = restClient.Get<University>("api/University");

            if (data != null)
            {
                univer = data.ToList();
            }
        }
    }
}
