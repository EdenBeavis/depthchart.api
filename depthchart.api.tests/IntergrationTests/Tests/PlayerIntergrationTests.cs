using depthchart.api.Infrastructure;
using depthchart.api.Models;
using depthchart.api.tests.IntergrationTests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;

namespace depthchart.api.tests.IntergrationTests.Tests
{
    public class PlayerIntergrationTests : IClassFixture<DepthChartTestsFixture>
    {
        private readonly DepthChartTestsFixture _fixture;

        public PlayerIntergrationTests(DepthChartTestsFixture fixture)
        {
            _fixture = fixture;
        }

        #region Given a user wants to add a player

        [Fact]
        public async Task When_The_Player_Id_Is_Greater_Than_Zero_Expect_BadRequest()
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

        [Fact]
        public async Task When_Name_Is_Empty_Expect_BadRequest()
        {
            // Arrange
            var player = new PlayerDto
            {
                Id = 0,
                Name = string.Empty,
                Number = 1,
            };

            // Act
            var (message, stringContent) = await _fixture.PostPlayer(player);

            //Assert
            var response = stringContent;

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Should().NotBeNullOrEmpty();
            response.Should().Contain(Constants.ErrorMessages.PlayerName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public async Task When_Number_Is_Less_Than_0_Or_Greater_Than_99_Expect_BadRequest(int number)
        {
            // Arrange
            var player = new PlayerDto
            {
                Id = 0,
                Name = "Test",
                Number = number,
            };

            // Act
            var (message, stringContent) = await _fixture.PostPlayer(player);

            //Assert
            var response = stringContent;

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Should().NotBeNullOrEmpty();
            response.Should().Contain(Constants.ErrorMessages.PlayerNumber);
        }

        [Fact]
        public async Task When_Details_Are_Correct_Then_Expect_Ok()
        {
            // Arrange
            var player = new PlayerDto
            {
                Id = 0,
                Name = "Test 1",
                Number = 1,
            };
            var expected = new PlayerDto
            {
                Id = 1,
                Name = "Test 1",
                Number = 1,
                Sport = Infrastructure.Enums.SportType.NFL
            };

            // Act
            var (message, stringContent) = await _fixture.PostPlayer(player);

            //Assert
            var response = JsonConvert.DeserializeObject<PlayerDto>(stringContent);

            message.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Should().BeEquivalentTo(expected);
        }

        #endregion Given a user wants to add a player
    }
}