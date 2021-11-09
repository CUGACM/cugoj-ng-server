using Microsoft.AspNetCore.Mvc;

namespace cugoj_ng_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        readonly int maxStatusPerPage;
        public StatusController(IConfiguration configuration)
        {
            maxStatusPerPage = configuration.GetValue<int>("Config:MaxStatusPerPage");
        }

        [HttpGet]
        public object List()
        {
            if (Request.Query.TryGetValue("cid", out var s))
            {
                if (s.Any(x => !int.TryParse(x, out _)))
                    return BadRequest("cid must be int");
            }
            return 1;
        }
    }
}
