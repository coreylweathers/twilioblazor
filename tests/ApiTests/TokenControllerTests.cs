using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using api.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using shared.Models;
using Xunit;

namespace tests.ApiTests
{
    public class TokenControllerTests
    {
        private TokenController _controller;

        private readonly Mock<ILogger<TokenController>> _loggerMock;
        private readonly Mock<IOptionsMonitor<TwilioAccountSettings>> _twilioSettingsMock;
        private readonly Mock<IOptionsMonitor<TwilioChatSettings>> _chatSettingsMock;

        public TokenControllerTests()
        {
            _loggerMock = new Mock<ILogger<TokenController>>();
            _chatSettingsMock = new Mock<IOptionsMonitor<TwilioChatSettings>>();
            _twilioSettingsMock = new Mock<IOptionsMonitor<TwilioAccountSettings>>();
        }


        [Fact]
        public async Task PostChatToken_WithNoIdentity_ReturnsValidToken()
        {
            // ARRANGE
            _chatSettingsMock.SetupGet(x => x.CurrentValue).Returns(new TwilioChatSettings { ApiKey = "SKd352567f538c47a59c8dc7c0ff1731da", ApiSecret = "1cdb7f65c7d845da87de5cd2e7477500", ServiceSid = "ISf1f1211e51c244ab9948f6e6a4a8b29b" });

            _twilioSettingsMock.SetupGet(x => x.CurrentValue).Returns(new TwilioAccountSettings { AccountSid = "AC603206310c6c40229cce6adb9c3ad18a", AuthToken = "37c04933621a437cb9ade0637a44d1ab" });

            _controller = new TokenController(_loggerMock.Object, _twilioSettingsMock.Object, _chatSettingsMock.Object);

            // ACT
            var result = await _controller.PostChatToken();

            // ASSERT
            Assert.True(!string.IsNullOrEmpty(result.ToString()));
        }

        [Fact]
        public async Task PostChatToken_WithInvalidSettings_ThrowsException()
        {
            // ARRANGE
            _chatSettingsMock.SetupGet(x => x.CurrentValue).Returns(new TwilioChatSettings { ApiKey = string.Empty, ApiSecret = string.Empty, ServiceSid = string.Empty });

            _twilioSettingsMock.SetupGet(x => x.CurrentValue).Returns(new TwilioAccountSettings { AccountSid = string.Empty, AuthToken = string.Empty });

            _controller = new TokenController(_loggerMock.Object, _twilioSettingsMock.Object, _chatSettingsMock.Object);

            // ACT
            // ASSERT
            await Assert.ThrowsAsync<ApplicationException>(async() => await _controller.PostChatToken());


        }
    }
}
