using AutoMapper;
using depthchart.api.Features.DepthChart.RequestHandlers;
using depthchart.api.Infrastructure;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Infrastructure.Enums;
using depthchart.api.Models;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;

namespace depthchart.api.tests.UnitTests.DepthChartRequests
{
    public class AddPlayerToDepthChartRequestUnitTests
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _mockContext;
        private readonly AddPlayerToDepthChartRequestHandler _sut;

        private Player _frankWalker = new Player
        {
            Id = 1,
            Name = "Frank Walker",
            Number = 1,
            Sport = SportType.NFL
        };

        private Player _stevenRunner = new Player
        {
            Id = 2,
            Name = "Steven Runner",
            Number = 2,
            Sport = SportType.NFL
        };

        private Player _michaelCrawler = new Player
        {
            Id = 3,
            Name = "Michael Crawler",
            Number = 5,
            Sport = SportType.NFL
        };

        public AddPlayerToDepthChartRequestUnitTests()
        {
            _mapper = AutoMapperHelper.CreateTestMapper();
            _mockContext = Create.MockedDbContextFor<DepthChartContext>();
            _sut = new AddPlayerToDepthChartRequestHandler(_mapper, _mockContext);

            _mockContext.Players.Add(_frankWalker);
            _mockContext.Players.Add(_stevenRunner);
            _mockContext.Players.Add(_michaelCrawler);

            _mockContext.SaveChanges();
        }

        #region Given_A_Player_Is_Being_Added_To_The_Depth_Chart

        [Fact]
        public async Task When_The_Player_Doesnt_Exist_Expect_Error_Message()
        {
            // Setup
            var expectedObject = new DepthPositionDto();
            const string expectedExceptionMessage = Constants.ErrorMessages.PlayerDoesNotExist;
            var request = new AddPlayerToDepthChartRequest("QB", 10, 0);

            // Action
            var depthPostionTask = await _sut.Handle(request, default);

            // Assert
            var deptbPostion = await depthPostionTask.Match(dp => dp, ex =>
            {
                ex.Message.Should().BeEquivalentTo(expectedExceptionMessage);
                return new();
            });

            deptbPostion.Should().BeEquivalentTo(expectedObject);
        }

        [Theory]
        [InlineData("", "The PlayerPosition field is required.")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", Constants.ErrorMessages.DepthPostionPosition)]
        public async Task When_The_Position_Is_Less_Than_1_Or_Greater_Than_255_Expect_Error_Message(string position, string errorMessage)
        {
            // Setup
            var expectedObject = new DepthPositionDto();
            var request = new AddPlayerToDepthChartRequest(position, 1, 0);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex =>
            {
                ex.Message.Should().BeEquivalentTo(errorMessage);
                return new();
            });

            depthPosition.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_Depth_Greater_Than_99_Expect_Error_Message()
        {
            // Setup
            var expectedObject = new DepthPositionDto();
            const string expectedExceptionMessage = Constants.ErrorMessages.DepthPostionDepth;
            var request = new AddPlayerToDepthChartRequest("QB", 1, 100);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex =>
            {
                ex.Message.Should().BeEquivalentTo(expectedExceptionMessage);
                return new();
            });

            depthPosition.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_Depth_Is_Less_Than_0_Expect_Depth_Position_Created_With_Default_Position()
        {
            // Setup
            var position = "QB";
            var expectedObject = new DepthPositionDto
            {
                PlayerPosition = position,
                Depth = 0,
                Player = _mapper.Map<PlayerDto>(_frankWalker)
            };
            var request = new AddPlayerToDepthChartRequest(position, 1, -1);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex => new());

            depthPosition.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_Is_Null_Expect_Depth_Position_Created_With_Highest_Depth_For_Position()
        {
            // Setup
            var position = "QB";

            var fwDp = new DepthPosition
            {
                Depth = 0,
                PlayerPosition = position,
                Player = _frankWalker,
                PlayerId = _frankWalker.Id
            };
            var srDp = new DepthPosition
            {
                Depth = 1,
                PlayerPosition = position,
                Player = _stevenRunner,
                PlayerId = _stevenRunner.Id
            };
            _mockContext.DepthPositions.Add(fwDp);
            _mockContext.DepthPositions.Add(srDp);

            await _mockContext.SaveChangesAsync();

            var expectedObject = new DepthPositionDto
            {
                PlayerPosition = position,
                Depth = 2,
                Player = _mapper.Map<PlayerDto>(_michaelCrawler)
            };
            var request = new AddPlayerToDepthChartRequest(position, 3, null);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex => new());

            depthPosition.Should().BeEquivalentTo(expectedObject);
            fwDp.Depth.Should().Be(0);
            srDp.Depth.Should().Be(1);
        }

        [Fact]
        public async Task When_Is_Null_And_There_Is_An_Empty_Slot_Above_It_Expect_Depth_Position_Created_With_Highest_Depth_For_Position()
        {
            // Setup
            var position = "QB";
            _mockContext.DepthPositions.Add(new DepthPosition
            {
                Depth = 1,
                PlayerPosition = position,
                Player = _stevenRunner,
                PlayerId = _stevenRunner.Id
            });

            await _mockContext.SaveChangesAsync();

            var expectedObject = new DepthPositionDto
            {
                PlayerPosition = position,
                Depth = 2,
                Player = _mapper.Map<PlayerDto>(_michaelCrawler)
            };
            var request = new AddPlayerToDepthChartRequest(position, 3, null);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex => new());

            depthPosition.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_The_Depth_Position_Is_Created_With_The_Same_Depth_Of_Other_Depth_Positions_Then_Create_Depth_Position_And_Move_Others_Up()
        {
            // Setup
            var position = "QB";
            var depth = 0;

            var fwDp = new DepthPosition
            {
                Depth = 0,
                PlayerPosition = position,
                Player = _frankWalker,
                PlayerId = _frankWalker.Id
            };
            var srDp = new DepthPosition
            {
                Depth = 1,
                PlayerPosition = position,
                Player = _stevenRunner,
                PlayerId = _stevenRunner.Id
            };
            _mockContext.DepthPositions.Add(fwDp);
            _mockContext.DepthPositions.Add(srDp);

            await _mockContext.SaveChangesAsync();

            var expectedObject = new DepthPositionDto
            {
                PlayerPosition = position,
                Depth = depth,
                Player = _mapper.Map<PlayerDto>(_michaelCrawler)
            };
            var request = new AddPlayerToDepthChartRequest(position, 3, depth);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex => new());

            depthPosition.Should().BeEquivalentTo(expectedObject);
            fwDp.Depth.Should().Be(1);
            srDp.Depth.Should().Be(2);
        }

        [Fact]
        public async Task When_Creating_Another_Depth_Position_With_Different_Positions_Then_New_Depth_Should_Be_At_Highest_For_Position()
        {
            // Setup
            var newPosition = "QB";

            var fwDp = new DepthPosition
            {
                Depth = 0,
                PlayerPosition = "FF",
                Player = _frankWalker,
                PlayerId = _frankWalker.Id
            };

            _mockContext.DepthPositions.Add(fwDp);

            await _mockContext.SaveChangesAsync();

            var expectedObject = new DepthPositionDto
            {
                PlayerPosition = newPosition,
                Depth = 0,
                Player = _mapper.Map<PlayerDto>(_michaelCrawler)
            };
            var request = new AddPlayerToDepthChartRequest(newPosition, 3, null);

            // Action
            var depthPositionTask = await _sut.Handle(request, default);

            // Assert
            var depthPosition = await depthPositionTask.Match(dp => dp, ex => new());

            depthPosition.Should().BeEquivalentTo(expectedObject);
            fwDp.Depth.Should().Be(0);
        }

        #endregion Given_A_Player_Is_Being_Added_To_The_Depth_Chart
    }
}