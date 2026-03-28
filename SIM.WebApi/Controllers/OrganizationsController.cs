using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIM.Application.Abstractions.Services;
using SIM.Application.ViewModels.Organizations;
using SIM.WebApi.Auth;

namespace SIM.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController(IOrganizationAppService organizationAppService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationViewModel vm,
        CancellationToken cancellationToken)
    {
        var result = await organizationAppService.CreateAsync(vm, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrStockManager)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await organizationAppService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await organizationAppService.GetAllAsync(cancellationToken);
        return Ok(result);
    }
}
