using Application.Services;
using AutoMapper;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Queries;

public class GetEmployeesQuery : IRequest<IResponseWrapper>
{
}

public class GetEmployeesQueryHandler(IEmployeeService employeeService,
    IMapper mapper) : IRequestHandler<GetEmployeesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetEmployeesAsync();
        return await ResponseWrapper<List<EmployeeResponse>>.SuccessAsync(
            mapper.Map<List<EmployeeResponse>>(employees));
    }
}




