using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using NodaTime;

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
            var eventResponse = await SendEvent(new KilometerEvent
            {
                Name = "user_signup",
                Type = "identified",
                UserId = userId,
            });

            AssertResponseIsValid(eventResponse);

            await UpdateUser(userId, userProperties);
        }

        public async Task UpdateUser(string userId, object userProperties)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, string.Format("/users/{0}/properties", userId))
            {
                Content = new ObjectContent(userProperties.GetType(), userProperties, new JsonMediaTypeFormatter())
            };

            AppendTimestamp(request);

            var response = await HttpClient.SendAsync(request);

            AssertResponseIsValid(response);
        }

        private void AppendTimestamp(HttpRequestMessage request)
        {
            request.Headers.Add("Timestamp", SystemClock.Instance.Now.Ticks.ToString());
        }

        private void AssertResponseIsValid(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new KilometerException("Impossible to connect to kilometer.io", response);
            }
        }

        public async Task CancelUser(string userId)
        {
            var eventResponse = await SendEvent(new KilometerEvent
            {
                UserId = userId,
                Name = "user_cancel",
                Type = "identified"
            });

            AssertResponseIsValid(eventResponse);

            await UpdateUser(userId, new
            {
                status = "Cancelled",
                paying_user = "no"
            });
        }

        public async Task BillUser(string userId, decimal amount)
        {
            var eventResponse = await SendEvent(new KilometerEvent
            {
                UserId = userId,
                Name = "user_billed",
                Type = "identified",
                Properties = new
                {
                    billed_amount = amount
                }
            });

            AssertResponseIsValid(eventResponse);

            var propertiesRequest = new HttpRequestMessage(HttpMethod.Post, string.Format(
                "/users/{0}/properties/revenue/increase/{1}",
                userId,
                amount.ToString("0.00", CultureInfo.InvariantCulture)));

            AppendTimestamp(propertiesRequest);

            var propertiesResponse = await HttpClient.SendAsync(propertiesRequest);

            AssertResponseIsValid(propertiesResponse);

            await UpdateUser(userId, new
            {
                paying_user = "yes",
                status = "active"
            });
        }

        private async Task<HttpResponseMessage> SendEvent(KilometerEvent evnt)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/events")
            {
                Content = new ObjectContent<KilometerEvent>(evnt, new JsonMediaTypeFormatter())
            };

            AppendTimestamp(request);

            return await HttpClient.SendAsync(request);
        }
    }
}
