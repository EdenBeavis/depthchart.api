using depthchart.api.Features.Players.RequestHandlers;
using depthchart.api.Infrastructure.Extensions;
using depthchart.api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace depthchart.api.Features.Players
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : Controller
    {
        private readonly IMediator _mediator;

        public PlayersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPlayer(PlayerDto newPlayer, CancellationToken token)
        {
            if (ModelState.ModelStateInvalidOrIsModelNull(newPlayer, out var errors))
                return BadRequest(errors);
            if (newPlayer.Id > 0)
                return BadRequest("New players must have an id equal to 0");

            var createPlayerTask = await _mediator.Send(new AddNewPlayerRequest(newPlayer), token);

            return await createPlayerTask.Match(
                player => Ok(player),
                ex => (IActionResult)BadRequest("Could not create player."));
        }
    }
}