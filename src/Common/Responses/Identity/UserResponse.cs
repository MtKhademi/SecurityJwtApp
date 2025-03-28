namespace Common.Responses.Identity;

public class UserResponse
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActivate { get; set; }
    public bool EmailConfirmed { get; set; }
}