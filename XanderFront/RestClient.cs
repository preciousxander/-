using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ClassDataBaseLibrary;

namespace XanderFront
{
    public class RestClient
    {
        public Uri baseAddress;
        private readonly HttpClient _client;
        private HttpResponseMessage resp;

        public RestClient(string uri) 
        {
            baseAddress = new Uri(uri);
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public string GetContent()
        {
            return resp.Content.ReadAsStringAsync().Result;
        }

        public bool IsOk()
        {
            return resp.IsSuccessStatusCode;
        }

        public void SetToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public bool Post<T>(string apiPath, T tableObject) where T : class
        {
            string data = JsonConvert.SerializeObject(tableObject);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            resp = _client.PostAsync(baseAddress + apiPath, content).Result;

            return IsOk();
        }

        public List<T> Get<T>(string apiPath) where T : class
        {
            List<T> list = new List<T>();
            resp = _client.GetAsync(baseAddress + apiPath).Result;
            
            if (IsOk())
            {
                string data = GetContent();
                list = JsonConvert.DeserializeObject<List<T>>(data);
            }

            return list;
        }

        public T Get<T>(string apiPath, int id) where T : class
        {
            T obj = null;
            resp = _client.GetAsync(baseAddress + apiPath + "/" + id).Result;

            if (IsOk())
            {
                string data = GetContent();
                obj = JsonConvert.DeserializeObject<T>(data);
            }

            return obj;
        }

        public bool Put<T>(string apiPath, T tableObject) where T : BaseModel
        {
            string data = JsonConvert.SerializeObject(tableObject);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            resp = _client.PutAsync(_client.BaseAddress + apiPath + "/" + tableObject.Id, content).Result;

            return IsOk();
        }

        public bool Delete(string apiPath, int id)
        {
            resp = _client.DeleteAsync(_client.BaseAddress + apiPath + "/" + id).Result;

            return IsOk();
        }
    }
}
