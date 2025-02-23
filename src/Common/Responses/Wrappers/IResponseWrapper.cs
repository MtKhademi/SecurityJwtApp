namespace Common.Responses.Wrappers;

public interface IResponseWrapper
{
    List<string> Messages { get; }
    bool IsSuccessful { get; }
}

public interface IResponseWrapper<out T> : IResponseWrapper
{
    T ResponseData { get; }
}