using Api.Attributes;
using Application.Features.Employees.Commands;
using Application.Features.Employees.Queries;
using Common.Authorization;
using Common.Requests.Employees;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class EmployeeController : MyBaseController<EmployeeController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Employees,AppActions.Create)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var response = await MediatorSender.Send(new CreateEmployeeCommand { EmployeeRequest = request });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }
    
    [HttpPut]
    [MustHavePermission(AppFeature.Employees,AppActions.Update)]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest request)
    {
        var response = await MediatorSender.Send(new UpdateEmployeeCommand() { UpdateEmployeeRequest = request });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }
    
    [HttpDelete("{employeeId}")]
    [MustHavePermission(AppFeature.Employees,AppActions.Delete)]
    public async Task<IActionResult> DeleteEmployee([FromRoute] int employeeId)
    {
        var response = await MediatorSender.Send(new DeleteEmployeeCommand() { EmployeeId = employeeId });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }
    
    [HttpGet]
    [MustHavePermission(AppFeature.Employees,AppActions.Read)]
    public async Task<IActionResult> GetEmployees()
    {
        var response = await MediatorSender.Send(new GetEmployeesQuery());
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }
}