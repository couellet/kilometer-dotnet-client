using System;
using System.Dynamic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kilometer
{
    public class KilometerClient
    {
        private string _appId;
        private HttpClient _client;

        public const string DEFAULT_API_ENDPOINT = "http://events.kilometer.io";

        public KilometerClient SetAppId(string appId)
        {
            _appId = appId;

            return this;
        }

        private HttpClient HttpClient
        {
            get
            {
                if (_client != null)
                {
                    return _client;
                }

                _client = new HttpClient
                {
                    BaseAddress = new Uri(DEFAULT_API_ENDPOINT)
                };

                _client.DefaultRequestHeaders
                    .TryAddWithoutValidation("Customer-App-Id", _appId);

                return _client;
            }
        }

        public async Task CreateUser(string userId, object userProperties)
        {
            await SendEvent(new KilometerEvent
            {
                Name = "user_signup",
                Type = "identified",
                UserId = userId,
            });

            await UpdateUser(userId, userProperties: userProperties);
        }

        public async Task UpdateUser(string userId, string status = "active", object userProperties = null)
        {
            dynamic props = userProperties ?? new ExpandoObject();
            props.status = status;

            var request = new HttpRequestMessage(HttpMethod.Put, string.Format("/users/{0}/properties", userId))
            {
                Content = new ObjectContent(props.GetType(), props, new JsonMediaTypeFormatter())
            };

            await HttpClient.SendAsync(request);
        }

        public async Task CancelUser(string userId)
        {
            await SendEvent(new KilometerEvent
            {
                UserId = userId,
                Name = "user_cancel",
                Type = "identified"
            });

            await UpdateUser(userId, userProperties: new
            {
                status = "Cancelled",
                paying_user = "no"
            });
        }

        public async Task BillUser(string userId, decimal amount)
        {
            await SendEvent(new KilometerEvent
            {
                UserId = userId,
                Name = "user_billed",
                Type = "identified",
                Properties = new
                {
                    billed_amount = amount
                }
            });

            var propertiesRequest = new HttpRequestMessage(HttpMethod.Post, string.Format(
                "/users/{0}/properties/revenue/increase/{1}",
                userId,
                amount.ToString("0.00", CultureInfo.InvariantCulture)));

            propertiesRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await HttpClient.SendAsync(propertiesRequest);

            await UpdateUser(userId, userProperties: new
            {
                paying_user = "yes",
                status = "active"
            });
        }

        private async Task SendEvent(KilometerEvent evnt)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/events")
            {
                Content = new ObjectContent<KilometerEvent>(evnt, new JsonMediaTypeFormatter())
            };

            await HttpClient.SendAsync(request);
        }
    }
}
