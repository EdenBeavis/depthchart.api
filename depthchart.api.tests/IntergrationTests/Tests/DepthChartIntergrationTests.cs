using depthchart.api.Models;
using depthchart.api.tests.IntergrationTests.Fixtures;
using FluentAssertions;

namespace depthchart.api.tests.IntergrationTests.Tests
{
    public class DepthChartIntergrationTests : IClassFixture<DepthChartTestsFixture>
    {
        private readonly DepthChartTestsFixture _fixture;

        public DepthChartIntergrationTests(DepthChartTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Adding_A_Player_Endpoint_Id_Greater_Than_Zero_Expect_BadRequest()
        {
            // Arrange
            var player = new PlayerDto
            {
                Id = 1,
                Name = "Test",
                Number = 1,
            };

            // Act
            var (message, stringContent) = await _fixture.PostPlayer(player);

            //Assert
            var response = stringContent;

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Should().NotBeNullOrEmpty();
            response.Should().Be("New players must have an id equal to 0");
        }
    }
}