using Xunit;
using shared.Services;

namespace tests.ServiceTests
{
    public class TwilioServiceTest
    {
        IBackendService _service;

        public TwilioServiceTest()
        {
            _service = new TwilioService();
        }

        [Fact]
        public void GetChatToken_ReturnsValidToken()
        {
            // ARRANGE

            // ACT
            var token = _service.GetChatToken();

            // ASSERT
            Assert.True(!string.IsNullOrEmpty(token));
        }
    }
}
