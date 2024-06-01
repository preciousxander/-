using ClassDataBaseLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;

namespace XanderFront.Pages
{
    public class StudentCardModel : PageModel
    {
        private RestClient restClient = new RestClient("https://localhost:7227");

        //public List<string> countries;
        public List<Country> countries;

        public void OnGet()
        {
            var data = restClient.Get<Country>("api/Country");

            if (data != null)
            {
                //countries = data.Select(i => i.CountryName).ToList();
                countries = data.ToList();
            }

        }

        //public async Task OnGetAsync()
        //{

        //}
    }
}
