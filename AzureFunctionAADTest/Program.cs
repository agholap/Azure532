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
        static string clientId = "{ClientID}";//AD AppID
        static string clientSecret = "{App Secret}";
        static string tenantId = "{Tenant Guid}";
        static string authUrl = "https://login.microsoftonline.com/{tenantGuid}/oauth2/token";
        static string authority = "https://login.microsoftonline.com/{tenantGuid}/";
        // The server base address
        static string baseUrl = "https://helloworldaad.azurewebsites.net/api/HelloWorld/AAD/Amol?code=8fFJY6K6QBoYikMtSL0UYx99gJZBiUaF0fSaoRINOOEpJ2VSSYlGDQ==";

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
            var result = await authContext.AcquireTokenAsync("https://helloworldaad.azurewebsites.net", clientCredential);
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
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add the Authorization header with the AccessToken.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                // create the URL string.
                string url = "";//string.Format("v1/Articles?page={0}&tags={1}", page, HttpUtility.UrlEncode(tags));

                // make the request
                HttpResponseMessage response = await client.GetAsync(url);

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
                postData.Add(new KeyValuePair<string, string>("resource", "https://helloworldaad.azurewebsites.net"));

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
