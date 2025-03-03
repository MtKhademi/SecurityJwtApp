using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Commands;

public class DeleteEmployeeCommand : IRequest<IResponseWrapper>
{
    public int EmployeeId { get; set; }
}


public class DeleteEmployeeCommandHandler(
    IEmployeeService employeeService) : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(request.EmployeeId);
        if(employee is null)
            return await ResponseWrapper.FailAsync("Employee not found");
        
       await employeeService.DeleteEmployeeAsync(employee);  
       return await ResponseWrapper.SuccessAsync("Employee deleted");
    }
}