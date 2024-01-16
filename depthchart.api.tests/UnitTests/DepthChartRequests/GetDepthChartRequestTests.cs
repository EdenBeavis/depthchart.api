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
    public class GetDepthChartRequestTests
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _mockContext;
        private readonly GetDepthChartRequestHandler _sut;

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

        public GetDepthChartRequestTests()
        {
            _mapper = AutoMapperHelper.CreateTestMapper();
            _mockContext = Create.MockedDbContextFor<DepthChartContext>();
            _sut = new GetDepthChartRequestHandler(_mapper, _mockContext);

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

        #region Given_A_User_Wants_To_See_A_Depth_Chart

        [Fact]
        public async Task When_The_Depth_Chart_Is_Empty_Then_Return_An_Empty_Depth_Chart()
        {
            // Setup
            _mockContext.RemoveRange(_mockContext.DepthPositions);
            await _mockContext.SaveChangesAsync();

            var expectedObject = Enumerable.Empty<DepthChartRowDto>();
            var request = new GetDepthChartRequest();

            // Action
            var depthChart = await _sut.Handle(request, default);

            // Assert
            depthChart.Should().BeEmpty();
            depthChart.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task When_The_Depth_Chart_Has_Players_In_Only_One_Position_Then_Return_Only_Those_Players()
        {
            // Setup
            _mockContext.RemoveRange(_mockContext.DepthPositions.Where(dp => dp.PlayerPosition == "FF"));
            await _mockContext.SaveChangesAsync();

            var expectedObject = new List<DepthChartRowDto>
            {
                new DepthChartRowDto
                {
                    Position = "QB",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player1),
                        _mapper.Map<PlayerDto>(_player2),
                        _mapper.Map<PlayerDto>(_player3)
                    }
                }
            };
            var request = new GetDepthChartRequest();

            // Action
            var depthChart = await _sut.Handle(request, default);

            // Assert
            depthChart.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
        }

        [Fact]
        public async Task When_The_Depth_Chart_Has_Multiple_Positions_Then_Return_All_Positions()
        {
            // Setup
            var expectedObject = new List<DepthChartRowDto>
            {
                new DepthChartRowDto
                {
                    Position = "FF",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player4),
                        _mapper.Map<PlayerDto>(_player5),
                        _mapper.Map<PlayerDto>(_player6)
                    }
                },
                new DepthChartRowDto
                {
                    Position = "QB",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player1),
                        _mapper.Map<PlayerDto>(_player2),
                        _mapper.Map<PlayerDto>(_player3)
                    }
                }
            };

            var request = new GetDepthChartRequest();

            // Action
            var depthChart = await _sut.Handle(request, default);

            // Assert
            depthChart.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
        }

        [Fact]
        public async Task When_The_Depth_Chart_Has_Players_In_Multiple_Positions_Then_Return_All_Positions_In_Multiple_Positions()
        {
            // Setup
            _mockContext.DepthPositions.Add(new DepthPosition
            {
                Player = _player3,
                PlayerPosition = "CC",
                Depth = 0
            });
            _mockContext.DepthPositions.Add(new DepthPosition
            {
                Player = _player4,
                PlayerPosition = "QB",
                Depth = 4
            });
            await _mockContext.SaveChangesAsync();

            var expectedObject = new List<DepthChartRowDto>
            {
                new DepthChartRowDto
                {
                    Position = "CC",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player3),
                    }
                },
                new DepthChartRowDto
                {
                    Position = "FF",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player4),
                        _mapper.Map<PlayerDto>(_player5),
                        _mapper.Map<PlayerDto>(_player6)
                    }
                },
                new DepthChartRowDto
                {
                    Position = "QB",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player1),
                        _mapper.Map<PlayerDto>(_player2),
                        _mapper.Map<PlayerDto>(_player3),
                        _mapper.Map<PlayerDto>(_player4),
                    }
                }
            };

            var request = new GetDepthChartRequest();

            // Action
            var depthChart = await _sut.Handle(request, default);

            // Assert
            depthChart.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());
        }

        [Fact]
        public async Task When_The_Depth_Chart_Is_Returned_Then_A_Player_Is_Removed_Expected_Chart_With_Removed_Player()
        {
            // Setup
            var expectedObject = new List<DepthChartRowDto>
            {
                new DepthChartRowDto
                {
                    Position = "FF",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player4),
                        _mapper.Map<PlayerDto>(_player5),
                        _mapper.Map<PlayerDto>(_player6)
                    }
                },
                new DepthChartRowDto
                {
                    Position = "QB",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player1),
                        _mapper.Map<PlayerDto>(_player2),
                        _mapper.Map<PlayerDto>(_player3)
                    }
                }
            };

            var request = new GetDepthChartRequest();

            // Action
            var depthChart = await _sut.Handle(request, default);

            // Assert
            depthChart.Should().BeEquivalentTo(expectedObject, opt => opt.WithStrictOrdering());

            // Setup2
            _mockContext.RemoveRange(_mockContext.DepthPositions.Where(dp => dp.PlayerId == _player6.Id));
            await _mockContext.SaveChangesAsync();

            var expectedObject2 = new List<DepthChartRowDto>
            {
                new DepthChartRowDto
                {
                    Position = "FF",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player4),
                        _mapper.Map<PlayerDto>(_player5)
                    }
                },
                new DepthChartRowDto
                {
                    Position = "QB",
                    Players = new List<PlayerDto>
                    {
                        _mapper.Map<PlayerDto>(_player1),
                        _mapper.Map<PlayerDto>(_player2),
                        _mapper.Map<PlayerDto>(_player3)
                    }
                }
            };

            // Action2
            var depthChart2 = await _sut.Handle(request, default);

            // Assert2
            depthChart2.Should().BeEquivalentTo(expectedObject2, opt => opt.WithStrictOrdering());
        }

        #endregion Given_A_User_Wants_To_See_A_Depth_Chart
    }
}