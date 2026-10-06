using AlertApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AlertApi;



[ApiController]
[Route("api/alerts/")]
public class AlertController : ControllerBase
{
    private IRepositoryAlert _repo;


    public AlertController(IRepositoryAlert repo)
    {
        _repo = repo;
    }

    [HttpGet("count_alerts_by_command")]
    public async Task<ActionResult<CountAlertsByCommandsRes>> GetCountAlertsByCommands()
    {
       return Ok(await _repo.GetCountAlertsByCommandsAsync());
    }

    [HttpGet("count_alerts_by_command_priorities")]
    public async Task<ActionResult<ICollection<CountAlertByComanndsPrioriryRes>>> GetCountAlertsByCommandsPriorities()
    {
        return Ok(await _repo.GetCountAlertsByCommandsPrioritiesAsync());
    }

    [HttpGet("count_alerts_by_command_status")]
    public async Task<ActionResult<ICollection<CountAlertByComanndsStatusRes>>> GetCountAlertsByCommandsStatus()
    {
        return Ok(await _repo.GetCountAlertsByCommandsStatusAsync());
    }



}
