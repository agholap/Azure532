using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureFunctionAADTest
{
    class Program
    {
        /// The client information used to get the OAuth Access Token from the server.
        static string clientId = "10185c1d-85a1-4ce4-9324-b529d4113d80";//AD AppID
        static string clientSecret = "SycFewOHCRF4slx1YVmZuoG92AHEBbUvuNkwEvXVYZ0=";
        static string tenantId = "e378defb-a919-462d-ad99-c101ccff4dc6";
        static string authUrl = "https://login.microsoftonline.com/e378defb-a919-462d-ad99-c101ccff4dc6/oauth2/token";
        static string authority = "https://login.microsoftonline.com/e378defb-a919-462d-ad99-c101ccff4dc6/";
        // The server base address
        static string baseUrl = "https://fnservertoserver.azurewebsites.net/api/GetDataFromCRM?name=amol";

        // this will hold the Access Token returned from the server.
        static string accessToken = null;
        private static AuthenticationContext authContext = null;
        private static ClientCredential clientCredential = null;

        static void Main(string[] args)
        {
                Console.WriteLine("Starting ...");

            DoIt().Wait();
        }
        private static async Task<string> WithADAL()
        {
            //using ADAL
            authContext = new AuthenticationContext(authority);
            clientCredential = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync("https://fnservertoserver.azurewebsites.net", clientCredential);
            return result.AccessToken;
        }
        /// <summary>
        /// This method does all the work to get an Access Token and read the first page of
        /// Articles from the server.
        /// </summary>
        /// <returns></returns>
        private static async Task<int> DoIt()
        {
            // Get the Access Token.
            accessToken = await GetAccessToken();
            //accessToken = await WithADAL();
            Console.WriteLine(accessToken != null ? "Got Token" : "No Token found");

            // Get the Articles
            Console.WriteLine();
            Console.WriteLine("------ New C# Articles ------");

            dynamic response = await GetHelloWorld();
            if (response.items != null)
            {
                var articles = (dynamic)response.items;
                foreach (dynamic article in articles)
                    Console.WriteLine("Title: {0}", article.title);
            }

            // Get the Articles
            Console.Read();
       
            return 0;
        }

        /// <summary>
        /// Gets the page of Articles.
        /// </summary>
        /// <param name="page">The page to get.</param>
        /// <param name="tags">The tags to filter the articles with.</param>
        /// <returns>The page of articles.</returns>
        private static async Task<dynamic> GetHelloWorld()
        {
            using (var client = new HttpClient())
            {
               // client.BaseAddress = new Uri(baseUrl);
               // client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add the Authorization header with the AccessToken.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // create the URL string.
                string url = "";//string.Format("v1/Articles?page={0}&tags={1}", page, HttpUtility.UrlEncode(tags));
                //HttpRequestMessage request = new HttpRequestMessage();
                //request.RequestUri = new Uri(baseUrl);
                
                // make the request
                HttpResponseMessage response = await client.GetAsync(baseUrl);

                // parse the response and return the data.
                string jsonString = await response.Content.ReadAsStringAsync();
                object responseData = JsonConvert.DeserializeObject(jsonString);
                return (dynamic)responseData;
            }
        }
        private static async Task<string> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(authUrl);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                // Build up the data to POST.
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                postData.Add(new KeyValuePair<string, string>("client_id", clientId));
                postData.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
                postData.Add(new KeyValuePair<string, string>("resource", "https://fnservertoserver.azurewebsites.net"));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                // Post to the Server and parse the response.
                HttpResponseMessage response = await client.PostAsync("", content);
                string jsonString = await response.Content.ReadAsStringAsync();
                object responseData = JsonConvert.DeserializeObject(jsonString);

                // return the Access Token.
                return ((dynamic)responseData).access_token;
            }
        }
    }
}
