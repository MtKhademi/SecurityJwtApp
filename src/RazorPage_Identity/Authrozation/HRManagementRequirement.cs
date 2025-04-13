using Microsoft.AspNetCore.Authorization;

namespace RazorPage_Identity.Authrozation;

public class HRManagementRequirement : IAuthorizationRequirement
{
    public int ProbationMonth { get; }

    public HRManagementRequirement(int probationMonth)
    {
        ProbationMonth = probationMonth;
    }
}

public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagementRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        HRManagementRequirement requirement)
    {
        if (!context.User.HasClaim(x => x.Type == "EmploymentDate" && x.Value != null))
            return Task.CompletedTask;
        var val = context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value;
        if (DateTime.TryParse(val, out DateTime employmentDate))
        {
            var probationEndDate = (DateTime.Now - employmentDate).Days;
            if (probationEndDate > 30 * requirement.ProbationMonth)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}