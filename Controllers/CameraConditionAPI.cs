using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/jobresults")]
public class JobResultController : ControllerBase
{
    private readonly IJobResultService _jobResultService;

    public JobResultController(IJobResultService jobResultService)
    {
        _jobResultService = jobResultService;
    }

    [HttpGet]
    public IActionResult GetResults()
    {
        var results = _jobResultService.GetResults();
        return Ok(results);
    }
}