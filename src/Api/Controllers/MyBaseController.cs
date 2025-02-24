using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class MyBaseController<T> : Controller
{
    private ISender _sender;
    public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
}