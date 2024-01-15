using Bolt.Common.Extensions;
using depthchart.api.Features.DepthChart.RequestHandlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace depthchart.api.Features.DepthChart
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepthChartController : Controller
    {
        private readonly IMediator _mediator;

        public DepthChartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPlayerToDepthChart(string playerPosition, int playerId, int? positionDepth, CancellationToken token)
        {
            if (playerId <= 0) return BadRequest("Player Id must be greater than 0.");
            if (playerPosition.IsEmpty()) return BadRequest("Player position must have a value.");

            var depthChartTask = await _mediator.Send(new AddNewPlayerToDepthChartRequest(playerPosition, playerId, positionDepth), token);

            return depthChartTask.Match(
                depthPosition => Ok(depthPosition),
                error => (IActionResult)BadRequest(error));
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePlayerFromDepthChart(string playerPosition, int playerId, CancellationToken token)
        {
            if (playerId <= 0) return BadRequest("Player Id must be greater than 0.");
            if (playerPosition.IsEmpty()) return BadRequest("Player position must have a value.");

            var playerRemovedInDepthPostion = await _mediator.Send(new RemovePlayerFromDepthChartRequest(playerPosition, playerId), token);

            return Ok(playerRemovedInDepthPostion);
        }

        [HttpGet]
        public async Task<IActionResult> GetBackups(string playerPosition, int playerId, CancellationToken token)
        {
            if (playerId <= 0) return BadRequest("Player Id must be greater than 0.");
            if (playerPosition.IsEmpty()) return BadRequest("Player position must have a value.");

            var playerBackups = await _mediator.Send(new GetBackupsForDepthChartRequest(playerPosition, playerId), token);

            return Ok(playerBackups);
        }

        [HttpGet]
        public async Task<IActionResult> GetFullDepthChart(CancellationToken token)
        {
            var depthChartRows = await _mediator.Send(new GetDepthChartRequest(), token);

            return Ok(depthChartRows);
        }
    }
}