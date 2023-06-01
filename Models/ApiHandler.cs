using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Elementary_schools_overview.Models
{
    class ApiHandler
    {
        private static readonly string apiBaseLink = "https://projekat.api.muharemovic.com/api/";
        public enum Http_Method
        {
            POST,
            GET
        }
        /* SendHttpRequest -> send a POST or GET request to a predefined API link
         * relativePathToApi -> path that will be added to apiBaseLink
         * method -> POST or GET method
         * formData -> additional data to send to API (for some POST requests) - NOT REQUIRED
         * return value is a string that contains json formatted data for further manipulation
         */
        public static string SendHttpRequest(string relativePathToApi, Http_Method method, ref bool isStatusCodeSuccess, MultipartFormDataContent formData = null)
        {
            string result = "";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Uri.EscapeUriString(apiBaseLink + relativePathToApi));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response;
            if (method == Http_Method.GET)
            {
                response = client.GetAsync(client.BaseAddress).Result;
            }
            else //it can only be POST
            {
                response = client.PostAsync(client.BaseAddress, formData).Result;
            }
            isStatusCodeSuccess = response.IsSuccessStatusCode;
            result = response.Content.ReadAsStringAsync().Result;
            result = result.Trim('[', ']'); //my JSON formatter is designed to work without these brackets
            return result;
        }

    }
}
