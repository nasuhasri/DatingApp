using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/users or /api/weatherForecast
    public class BaseApiController : ControllerBase
    {
        
    }
}