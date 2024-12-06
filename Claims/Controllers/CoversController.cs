using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Claims.Audit;
using Claims.Audit.Models;
using Claims.Models;
using Claims.PremiumCalculator;
using Claims.Services;
using Claims.Validators;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly IAuditService _auditService;
    private readonly ICoverService _coverService;
    private readonly CoverValidator _coverValidator;
    private readonly IPremiumCalculator _premiumCalculator;

    public CoversController(ILogger<CoversController> logger, ICoverService coverService, IAuditService auditService, 
        CoverValidator coverValidator, IPremiumCalculator premiumCalculator)
    {
        _logger = logger;
        _coverService = coverService;
        _auditService = auditService;
        _coverValidator = coverValidator;
        _premiumCalculator = premiumCalculator;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Cover>), StatusCodes.Status200OK)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Get a list of covers.")]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coverService.GetCoversAsync();
        return Ok(results);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Cover), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Get a cover with the given id.")]
    public async Task<ActionResult<Cover>> GetAsync([FromRoute] [Required] string id)
    {
        var result = await _coverService.GetCoverAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Cover), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Create a new cover.")]
    public async Task<ActionResult> CreateAsync([FromBody] Cover cover)
    {
        var validationResult = await _coverValidator.ValidateAsync(cover);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        await _coverService.AddCoverAsync(cover);
        await _auditService.AuditCover(cover.Id, AuditType.Post);
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Delete a cover with the given id.", Description = 
        """
        Create an new cover. A cover can not have a start date in the past and the total 
        insurance period cannot exceed 1 year.
        """)]
    public async Task<ActionResult> DeleteAsync([FromRoute] [Required] string id)
    {
        await _auditService.AuditCover(id, AuditType.Delete);
        await _coverService.DeleteCoverAsync(id);
        return NoContent();
    }
    
    [HttpPost("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Compute the premium for a cover.", Description = 
        """
        The total cost depends on the type of ship and the duration of the insurance policy.
        """)]
    public Task<ActionResult<decimal>> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = _premiumCalculator.Compute(startDate, endDate, coverType);
        return Task.FromResult<ActionResult<decimal>>(Ok(result));
    }
}
