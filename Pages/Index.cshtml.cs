using ConsumingRestWithANC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsumingRestWithANC.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private static readonly string api = "https://catfact.ninja/"; 
     
        static HttpClient? Client;
        public static string Message { get; set; } = "PageMode in C#";
        
        // The object that will hold the cat fact for the view/page in this context
        public static CatFact? catFact;

        /// <summary>
        /// Asyncronously gets the cat fact from the api path and stores it in the catFact object
        /// Modifies the CatFact object
        /// Eventually this function will moved to the a client class
        /// </summary>
        /// <param name="path">The path of the api</param>
        /// <returns>Task i.e. Ends the Thread</returns>
        static async Task GetCatFactAsync(string path)
        {
            // Send your get request
            HttpResponseMessage response = await Client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                catFact = await response.Content.ReadFromJsonAsync<CatFact>();
            } else
            {
                Message = "Sth is wrong in GetCatFactAsync()";
            }
        }

        public IndexModel(ILogger<IndexModel> logger)
        {
           
            Client = new HttpClient();
            _logger = logger;
        }

        // <summary>
        // Asynchrononusly executes the method that executes the thread to talk to the api
        // </summary>
        // <returns>The task: it's end</returns>
        static async Task RunAsync()
        {
            if (!Client.Equals(null))
            {
                // Set the base address of the api
                Client.BaseAddress = new Uri(api);
                // Remove all default headers
                Client.DefaultRequestHeaders.Accept.Clear();
                // We are expect a JSON response
                Client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    await GetCatFactAsync("fact");
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        public void OnGet()
        {
            // Aka on load :)
            RunAsync().GetAwaiter().GetResult();

            ViewData["quester"] = Message;
            _logger.Log(LogLevel.Information, "Done with Get");
        }
    }
}