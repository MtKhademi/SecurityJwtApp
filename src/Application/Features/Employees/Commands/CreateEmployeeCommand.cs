using Application.Services;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Employees.Commands;

public class CreateEmployeeCommand : IRequest<IResponseWrapper>
{
    public CreateEmployeeRequest EmployeeRequest { get; set; }
}

public class CreateEmployeeCommandHandler(
    IEmployeeService employeeService,
    IMapper mapper) : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var mappedEmployee = mapper.Map<Employee>(request.EmployeeRequest);
        var newEmployee = await employeeService.CreateEmployeeAsync(mappedEmployee);

        if (newEmployee.Id > 0)
        {
            var mappedEmployeeResponse = mapper.Map<EmployeeResponse>(newEmployee);
            return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedEmployeeResponse,"Employee created successfully");
        }
        
        return await ResponseWrapper<EmployeeResponse>.FailAsync("Employee could not be created");
    }
}