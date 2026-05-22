using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NashAssetManagement.WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assignments")]
    public class AssignmentsController
        : BaseApiController
    {
        readonly ILogger<AssignmentsController> _logger;

        public AssignmentsController(
            ISender sender,
            ILogger<AssignmentsController> logger) : base(sender)
        {
            _logger = logger;
        }
    }
}
