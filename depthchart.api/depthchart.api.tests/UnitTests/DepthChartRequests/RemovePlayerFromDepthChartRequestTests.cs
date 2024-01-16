using AutoMapper;
using depthchart.api.Features.DepthChart.RequestHandlers;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Infrastructure.Enums;
using depthchart.api.Models;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace depthchart.api.tests.UnitTests.DepthChartRequests
{
    public class RemovePlayerFromDepthChartRequestTests
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _mockContext;

        private readonly RemovePlayerFromDepthChartRequestHandler _sut;

        private Player _player1 = new Player
        {
            Id = 1,
            Name = "Player 1",
            Number = 1,
            Sport = SportType.NFL
        };

        private Player _player2 = new Player
        {
            Id = 2,
            Name = "Player 2",
            Number = 77,
            Sport = SportType.NFL
        };

        private Player _player3 = new Player
        {
            Id = 3,
            Name = "Player 3",
            Number = 5,
            Sport = SportType.NFL
        };

        private Player _player4 = new Player
        {
            Id = 4,
            Name = "Player 4",
            Number = 73,
            Sport = SportType.NFL
        };

        private Player _player5 = new Player
        {
            Id = 5,
            Name = "player5",
            Number = 8,
            Sport = SportType.NFL
        };

        private Player _player6 = new Player
        {
            Id = 6,
            Name = "player6",
            Number = 22,
            Sport = SportType.NFL
        };

        public RemovePlayerFromDepthChartRequestTests()
        {
            _mapper = AutoMapperHelper.CreateTestMapper();
            var logger = new Mock<ILogger<RemovePlayerFromDepthChartRequestHandler>>();
            _mockContext = Create.MockedDbContextFor<DepthChartContext>();
            _sut = new RemovePlayerFromDepthChartRequestHandler(_mapper, _mockContext, logger.Object);

            _mockContext.Players.Add(_player1);
            _mockContext.Players.Add(_player2);
            _mockContext.Players.Add(_player3);
            _mockContext.Players.Add(_player4);
            _mockContext.Players.Add(_player5);
            _mockContext.Players.Add(_player6);

            _mockContext.SaveChanges();

            var quarterBack = "QB";
            var fullForward = "FF";
            var p1Dp = new DepthPosition
            {
                Depth = 0,
                PlayerPosition = quarterBack,
                Player = _player1,
                PlayerId = _player1.Id
            };
            var p2Dp = new DepthPosition
            {
                Depth = 1,
                PlayerPosition = quarterBack,
                Player = _player2,
                PlayerId = _player2.Id
            };
            var p3Dp = new DepthPosition
            {
                Depth = 2,
                PlayerPosition = quarterBack,
                Player = _player3,
                PlayerId = _player3.Id
            };
            var p4Dp = new DepthPosition
            {
                Depth = 0,
                PlayerPosition = fullForward,
                Player = _player4,
                PlayerId = _player4.Id
            };
            var p5Dp = new DepthPosition
            {
                Depth = 1,
                PlayerPosition = fullForward,
                Player = _player5,
                PlayerId = _player5.Id
            };
            var p6Dp = new DepthPosition
            {
                Depth = 2,
                PlayerPosition = fullForward,
                Player = _player6,
                PlayerId = _player6.Id
            };
            _mockContext.DepthPositions.Add(p1Dp);
            _mockContext.DepthPositions.Add(p2Dp);
            _mockContext.DepthPositions.Add(p3Dp);
            _mockContext.DepthPositions.Add(p4Dp);
            _mockContext.DepthPositions.Add(p5Dp);
            _mockContext.DepthPositions.Add(p6Dp);

            _mockContext.SaveChanges();
        }

        #region Given_A_User_Wants_To_Remove_A_Player_From_The_Depth_Chart

        [Fact]
        public async Task When_The_Depth_Chart_Is_Empty_Then_Return_An_Empty_Player_List()
        {
            // Setup
            _mockContext.RemoveRange(_mockContext.DepthPositions);
            await _mockContext.SaveChangesAsync();

            var expectedObject = Enumerable.Empty<DepthChartRowDto>();
            var request = new RemovePlayerFromDepthChartRequest("QB", _player1.Id);

            // Action
            var depthChart = await _sut.Handle(request, default);

            // Assert
            depthChart.Should().BeEmpty();
            depthChart.Should().BeEquivalentTo(expectedObject);
        }

        [Theory]
        [InlineData("QB", 100)] // Position exist but Player does not exist
        [InlineData("CC", 1)] // Position doesn't exist but player does
        [InlineData("QB", 4)] // Position exist but the player isn't in that position
        public async Task When_the_Expected_Result_Is_An_Empty_List(string position, int playerId)
        {
            // Setup
            var expectedObject = Enumerable.Empty<PlayerDto>();
            var request = new RemovePlayerFromDepthChartRequest(position, playerId);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEmpty();
            players.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_A_Player_Is_Removed_From_The_Depth_Chart_Return_The_Removed_Player()
        {
            // Setup
            var expectedObject = new List<PlayerDto> { _mapper.Map<PlayerDto>(_player1) };

            var request = new RemovePlayerFromDepthChartRequest("QB", _player1.Id);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
            _mockContext.DepthPositions.Count().Should().Be(5);
        }

        [Fact]
        public async Task When_The_Depth_Chart_Has_Players_In_Multiple_Positions_Then_Remove_Only_The_Player_In_The_Specified_Position()
        {
            // Setup
            _mockContext.DepthPositions.Add(new DepthPosition
            {
                Player = _player3,
                PlayerPosition = "CC",
                Depth = 0
            });
            await _mockContext.SaveChangesAsync();

            var expectedObject = new List<PlayerDto> { _mapper.Map<PlayerDto>(_player3) };

            var request = new RemovePlayerFromDepthChartRequest("QB", _player3.Id);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
            _mockContext.DepthPositions.Count().Should().Be(6);
            _mockContext.DepthPositions.Where(dp => dp.PlayerPosition == "CC").Count().Should().Be(1);
        }

        #endregion Given_A_User_Wants_To_Remove_A_Player_From_The_Depth_Chart
    }
}