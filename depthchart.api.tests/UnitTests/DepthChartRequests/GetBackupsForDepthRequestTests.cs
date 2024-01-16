using AutoMapper;
using depthchart.api.Features.DepthChart.RequestHandlers;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Infrastructure.Enums;
using depthchart.api.Models;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;

namespace depthchart.api.tests.UnitTests.DepthChartRequests
{
    public class GetBackupsForDepthRequestTests
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _mockContext;
        private readonly GetBackupsForDepthChartRequestHandler _sut;

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

        public GetBackupsForDepthRequestTests()
        {
            _mapper = AutoMapperHelper.CreateTestMapper();
            _mockContext = Create.MockedDbContextFor<DepthChartContext>();
            _sut = new GetBackupsForDepthChartRequestHandler(_mapper, _mockContext);

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

        #region Given_A_User_Used_Position_To_Get_The_Backups_For_A_Player

        [Theory]
        [InlineData("QB", 100)] // Player does not exist
        [InlineData("QB", 3)] // Player with highest depth
        [InlineData("CC", 1)] // Position doesn't exist
        [InlineData("QB", 4)] // Position the player isn't in
        public async Task When_the_Expected_Result_Is_An_Empty_List(string position, int playerId)
        {
            // Setup
            var expectedObject = Enumerable.Empty<PlayerDto>();
            var request = new GetBackupsForDepthChartRequest(position, playerId);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEmpty();
            players.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_Getting_Backups_At_With_Lowest_Depth_Expect_Two_Players_Returned()
        {
            // Setup
            var expectedObject = new List<PlayerDto> { _mapper.Map<PlayerDto>(_player5), _mapper.Map<PlayerDto>(_player6) };
            var request = new GetBackupsForDepthChartRequest("FF", _player4.Id);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
        }

        [Fact]
        public async Task When_Getting_Backups_At_With_Second_Lowest_Depth_Expect_One_Player_Returned()
        {
            // Setup
            var expectedObject = new List<PlayerDto> { _mapper.Map<PlayerDto>(_player3) };
            var request = new GetBackupsForDepthChartRequest("QB", _player2.Id);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
        }

        [Fact]
        public async Task When_Getting_Backups_At_With_Lowest_Depth_Expect_Two_Players_Returned_Then_If_One_Is_Deleted_Return_One()
        {
            // Setup
            var expectedObject = new List<PlayerDto> { _mapper.Map<PlayerDto>(_player5), _mapper.Map<PlayerDto>(_player6) };
            var request = new GetBackupsForDepthChartRequest("FF", _player4.Id);

            // Action
            var players = await _sut.Handle(request, default);

            // Assert
            players.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());

            // Setup 2
            _mockContext.RemoveRange(_mockContext.DepthPositions.Where(dp => dp.PlayerId == _player5.Id));
            await _mockContext.SaveChangesAsync();
            var expectedObject2 = new List<PlayerDto> { _mapper.Map<PlayerDto>(_player6) };

            // Action
            var players2 = await _sut.Handle(request, default);

            // Assert
            players2.Should().BeEquivalentTo(expectedObject2, opt => opt.WithStrictOrdering());
        }

        #endregion Given_A_User_Used_Position_To_Get_The_Backups_For_A_Player
    }
}