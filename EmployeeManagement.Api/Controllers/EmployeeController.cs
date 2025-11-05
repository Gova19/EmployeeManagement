using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeController(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _repo.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return emp == null ? NotFound() : Ok(emp);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
        {
            var emp = new Employee(dto.FirstName, dto.LastName, dto.Email, dto.JobTitle);
            var result = await _repo.AddAsync(emp);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null) return NotFound();

            emp.UpdateJobTitle(dto.JobTitle);
            await _repo.UpdateAsync(emp);

            return Ok(emp);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _repo.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }

    // DTOs for request validation
    public record CreateEmployeeDto(string FirstName, string LastName, string Email, string JobTitle);
    public record UpdateEmployeeDto(string JobTitle);
}
