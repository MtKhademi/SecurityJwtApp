using System.Diagnostics;

namespace Common.Responses;

public class EmployeeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
}