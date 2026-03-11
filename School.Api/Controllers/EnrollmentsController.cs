using School.Application.Exceptions;
using School.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using School.Application.Abstractions;

namespace School.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(Guid studentId, Guid courseId)
    {
        try
        {
            await _enrollmentService.EnrollStudentAsync(studentId, courseId);
            return NoContent();
        }
        catch (DomainValidationException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
        catch (BusinessLogicException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _enrollmentService.UnenrollAsync(id);
            return NoContent();
        }
        catch (BusinessLogicException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        try
        {
            var enrollments = await _enrollmentService.GetByStudentAsync(studentId);
            return Ok(enrollments);
        }
        catch (BusinessLogicException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        try
        {
            var enrollments = await _enrollmentService.GetByCourseAsync(courseId);
            return Ok(enrollments);
        }
        catch (BusinessLogicException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
