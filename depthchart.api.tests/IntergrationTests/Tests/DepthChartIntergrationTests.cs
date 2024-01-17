using depthchart.api.Infrastructure.Enums;
using depthchart.api.Models;
using depthchart.api.tests.IntergrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace depthchart.api.tests.IntergrationTests.Tests
{
    public class DepthChartIntergrationTests : IClassFixture<DepthChartTestsFixture>
    {
        private readonly DepthChartTestsFixture _fixture;

        public DepthChartIntergrationTests(DepthChartTestsFixture fixture)
        {
            _fixture = fixture;

            var scope = _fixture.Services.CreateScope();

            var dataProvider = scope.ServiceProvider.GetRequiredService<RefDataProvider>();

            dataProvider.SetupRefData();
        }

        #region Given a user wants to add a player to a depthchart

        [Fact]
        public async Task When_The_Player_Position_Is_Empty_Zero_Expect_BadRequest()
        {
            // Arrange
            var position = "";
            var playerId = 1;
            int? depth = null;

            // Act
            var (message, stringContent) = await _fixture.PostDepthChart(position, playerId, depth);

            //Assert
            var response = stringContent;

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Should().NotBeNullOrEmpty();
            response.Should().Contain("The playerPosition field is required.");
        }

        [Fact]
        public async Task When_The_Player_Id_Is_Zero_Or_Less_Expect_BadRequest()
        {
            // Arrange
            var position = "QB";
            var playerId = 0;
            int? depth = null;

            // Act
            var (message, stringContent) = await _fixture.PostDepthChart(position, playerId, depth);

            //Assert
            var response = stringContent;

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Should().NotBeNullOrEmpty();
            response.Should().Contain("Player Id must be greater than 0.");
        }

        [Fact]
        public async Task When_The_Details_Are_Correct_Expect_Ok()
        {
            // Arrange
            var position = "QB";
            var playerId = 1;
            int? depth = null;

            var expcted = new DepthPositionDto
            {
                Depth = 0,
                PlayerPosition = position,
                Player = new PlayerDto
                {
                    Id = playerId,
                    Name = "Test 1",
                    Number = 1,
                    Sport = SportType.NFL
                }
            };

            // Act
            var (message, stringContent) = await _fixture.PostDepthChart(position, playerId, depth);

            //Assert
            var response = JsonConvert.DeserializeObject<DepthPositionDto>(stringContent);

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Should().BeEquivalentTo(expcted);
        }

        #endregion Given a user wants to add a player to a depthchart
    }
}