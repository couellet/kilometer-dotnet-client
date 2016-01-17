using System.Threading.Tasks;
using Kilometer;
using NUnit.Framework;

namespace KilometerTests
{
    [Ignore("Integration tests only.")]
    public class KilometerClientTests
    {
        public KilometerClient GetClient()
        {
            return new KilometerClient().SetAppId("APP_ID");
        }

        [Test]
        public async Task CreateUser_ShouldWOrk()
        {
            await GetClient().CreateUser("user_id", new {Fee = 0.05});
        }

        [Test]
        public async Task UpdateUser_ShouldWork()
        {
            await GetClient().UpdateUser("user_id", new
            {
                Meta = "Test"
            });
        }

        [Test]
        public async Task BillUser_ShouldWork()
        {
            await GetClient().BillUser("user_id", 20);
        }

        [Test]
        public async Task CancelUser_ShouldWork()
        {
            await GetClient().CancelUser("user_id");
        }
    }
}