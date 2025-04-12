namespace Common.Requests.Identity;

public class ChangeUserStatusRequest
{
    public string UserId { get; set; }
    public bool Active { get; set; }
}