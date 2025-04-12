namespace Infrastructure.Extentions;

public static class IdentityResultExtentions
{
    public static List<string> GetErrors(this IdentityResult identityResult)
    {
        return identityResult.Errors.Select(er => er.Description).ToList();
    }
}