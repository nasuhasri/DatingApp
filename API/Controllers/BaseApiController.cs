using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    // tell the system to find the request body from the body
    // automatically check the validation before it even get to the controller
    [ApiController]
    [Route("api/[controller]")] // /api/users or /api/weatherForecast
    public class BaseApiController : ControllerBase
    {
        
    }
}