using Application.Services;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Commands;

public class UpdateEmployeeCommand : IRequest<IResponseWrapper>
{
    public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; }
}

public class UpdateEmployeeCommandHandler(
    IEmployeeService employeeService,
    IMapper mapper) : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeInDb = await employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id);
        if (employeeInDb is null)
            return ResponseWrapper.Fail("Employee not found");

        employeeInDb.FirstName = request.UpdateEmployeeRequest.FirstName;
        employeeInDb.LastName = request.UpdateEmployeeRequest.LastName;
        employeeInDb.Email = request.UpdateEmployeeRequest.Email;
        employeeInDb.Salary = request.UpdateEmployeeRequest.Salary;

        await employeeService.UpdateEmployeeAsync(employeeInDb);

        return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mapper.Map<EmployeeResponse>(employeeInDb));
    }
}