using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kilometer
{
    public class KilometerException : Exception
    {
        public HttpResponseMessage Response { get; set; }

        public KilometerException(string message, HttpResponseMessage response = null) : base(message)
        {
            Response = response;
        }

        public KilometerException(string message, Exception innerException, HttpResponseMessage response = null) : base(message, innerException)
        {
            Response = response;
        }

        public async Task<object> GetResponseContent()
        {
            if (Response != null &&
                Response.Content.Headers.ContentType.MediaType == "application/json")
            {
                var response = await Response.Content.ReadAsStringAsync();

                return JsonConvert.SerializeObject(response);
            }

            return null;
        }
    }
}