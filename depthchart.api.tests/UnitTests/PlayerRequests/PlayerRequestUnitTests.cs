using depthchart.api.Features.Players.RequestHandlers;
using depthchart.api.Infrastructure;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Infrastructure.Enums;
using depthchart.api.Models;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;

namespace depthchart.api.tests.UnitTests.PlayerRequests
{
    public class PlayerRequestUnitTests
    {
        private readonly DepthChartContext _mockContext;
        private readonly AddNewPlayerRequestHandler _sut;

        public PlayerRequestUnitTests()
        {
            var mapper = AutoMapperHelper.CreateTestMapper();
            _mockContext = Create.MockedDbContextFor<DepthChartContext>();
            _sut = new AddNewPlayerRequestHandler(mapper, _mockContext);
        }

        #region Given_A_Player_Is_Being_Added

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public async Task When_The_Player_Number_Is_Than_0_Or_Greater_Than_99_Expected_An_Exception_Thrown(int playerNumber)
        {
            // Setup
            var expectedObject = new PlayerDto();
            const string expectedExceptionMessage = Constants.ErrorMessages.PlayerNumber;

            var newPlayer = new PlayerDto
            {
                Name = "Frank Walker",
                Number = playerNumber
            };
            var request = new AddNewPlayerRequest(newPlayer);

            // Action
            var playerTask = await _sut.Handle(request, default);

            // Assert
            var player = await playerTask.Match(player => player, ex =>
            {
                ex.Message.Should().BeEquivalentTo(expectedExceptionMessage);
                return new();
            });

            player.Should().BeEquivalentTo(expectedObject);
        }

        [Theory]
        [InlineData("", "The Name field is required.")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", Constants.ErrorMessages.PlayerName)]
        public async Task When_The_Player_Name_Is_Less_Than_1_Or_Greater_Than_255_Expect_Error_Message(string name, string errorMessage)
        {
            // Setup
            var expectedObject = new PlayerDto();

            var newPlayer = new PlayerDto
            {
                Name = name,
                Number = 50
            };
            var request = new AddNewPlayerRequest(newPlayer);

            // Action
            var playerTask = await _sut.Handle(request, default);

            // Assert
            var player = await playerTask.Match(player => player, ex =>
            {
                ex.Message.Should().BeEquivalentTo(errorMessage);
                return new();
            });

            player.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_All_Data_Is_Correct_Expect_Created_Player_Returned()
        {
            // Setup
            var expected = new PlayerDto
            {
                Id = 1,
                Name = "Frank Walker",
                Number = 1,
                Sport = SportType.NFL
            };

            var newPlayer = new PlayerDto
            {
                Name = "Frank Walker",
                Number = 1
            };
            var request = new AddNewPlayerRequest(newPlayer);

            // Action
            var playerTask = await _sut.Handle(request, default);

            // Assert
            var player = await playerTask.Match(player => player, ex => new());

            player.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task When_Other_Are_Players_Created_Expect_Created_Player_Returned()
        {
            // Setup
            for (var i = 1; i <= 10; i++)
                _mockContext.Players.Add(new Player
                {
                    Name = $"Test {i}",
                    Number = i,
                    Sport = SportType.NFL
                });

            await _mockContext.SaveChangesAsync();

            var expected = new PlayerDto
            {
                Id = 11,
                Name = "Frank Walker",
                Number = 99,
                Sport = SportType.NFL
            };

            var newPlayer = new PlayerDto
            {
                Name = "Frank Walker",
                Number = 99
            };
            var request = new AddNewPlayerRequest(newPlayer);

            // Action
            var playerTask = await _sut.Handle(request, default);

            // Assert
            var player = await playerTask.Match(player => player, ex => new());

            player.Should().BeEquivalentTo(expected);
        }

        #endregion Given_A_Player_Is_Being_Added
    }
}