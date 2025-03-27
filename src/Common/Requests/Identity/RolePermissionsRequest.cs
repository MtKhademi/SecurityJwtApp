using Common.Responses.Identity;

namespace Common.Requests.Identity;

public class RolePermissionsRequest
{
    public string RoleId { get; set; }
    public List<RoleClaimViewModel> Type { get; set; }
}