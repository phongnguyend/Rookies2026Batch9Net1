using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NashAssetManagement.WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseApiController
        : ControllerBase
    {
        protected readonly ISender _sender;

        protected BaseApiController(ISender sender)
        {
            _sender = sender;
        }
    }
}
