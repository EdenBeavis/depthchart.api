using AutoMapper;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.Players.RequestHandlers
{
    public class GetAllPlayersRequest : IRequest<IEnumerable<PlayerDto>>
    {
    }

    public class GetAllPlayersRequestHandler : IRequestHandler<GetAllPlayersRequest, IEnumerable<PlayerDto>>
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _dbContext;

        public GetAllPlayersRequestHandler(IMapper mapper, DepthChartContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PlayerDto>> Handle(GetAllPlayersRequest request, CancellationToken token)
        {
            var players = await _dbContext.Players.ToListAsync(token);

            return _mapper.Map<IEnumerable<PlayerDto>>(players);
        }
    }
}