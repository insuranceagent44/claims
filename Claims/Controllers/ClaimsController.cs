using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Claims.Audit;
using Claims.Audit.Models;
using Claims.Models;
using Claims.Services;
using Claims.Validators;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ILogger<ClaimsController> _logger;
    private readonly IAuditService _auditService;
    private readonly IClaimService _claimService;
    private readonly ClaimValidator _claimsValidator;

    public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService, IAuditService auditService, 
        ClaimValidator claimsValidator)
    {
        _logger = logger;
        _claimService = claimService;
        _auditService = auditService;
        _claimsValidator = claimsValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Claim>), StatusCodes.Status200OK)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Get a list of claims.")]
    public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
    {
        var results = await _claimService.GetClaimsAsync();
        return Ok(results);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Claim), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Get a claim with the given id.")]
    public async Task<ActionResult<Claim>> GetAsync([FromRoute] [Required] string id)
    {
        var result = await _claimService.GetClaimAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Claim), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = "Create a new claim.", Description = 
        """
        Create an new claim against a cover. A claim can not exceed 100000 and must 
        be created within the cover period.
        """)]
    public async Task<ActionResult> CreateAsync([FromBody] Claim claim)
    {
        var validationResult = await _claimsValidator.ValidateAsync(claim);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        await _claimService.AddClaimAsync(claim);
        await _auditService.AuditClaim(claim.Id, AuditType.Post);
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Delete a claim with the given id.")]
    public async Task<ActionResult> DeleteAsync([FromRoute] [Required] string id)
    {
        await _auditService.AuditClaim(id, AuditType.Delete);
        await _claimService.DeleteClaimAsync(id);
        return NoContent();
    }
}