using cugoj_ng_server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace cugoj_ng_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        readonly int problemsPerPage;
        readonly int maxSolutionSize;
        public ProblemController(IConfiguration configuration)
        {
            problemsPerPage = configuration.GetValue<int>("Config:ProblemsPerPage");
            maxSolutionSize = configuration.GetValue<int>("Config:MaxSolutionSize");
        }

        [HttpGet]
        [Route("List")]
        public async Task<object> GetProblemCountAsync()
        {
            var viewAll = await UserModel.Authorization.CanViewAllProblemsAsync(HttpContext.Session.GetString("user"));
            var problemCount = await ProblemModel.GetProblemsCountAsync(viewAll);
            var maxPageCount = (problemCount + problemsPerPage - 1) / problemsPerPage;
            return new
            {
                TotalPages = maxPageCount,
                ProblemCount = problemCount,
            };
        }

        [HttpGet]
        [Route("List/{page:int:min(1)}")]
        public async Task<object> GetProblemListAsync(int page)
        {
            if (page <= 0) return BadRequest();
            bool viewAll = await UserModel.Authorization.CanViewAllProblemsAsync(HttpContext.Session.GetString("user"));
            int maxPageCount = (await ProblemModel.GetProblemsCountAsync(viewAll) + problemsPerPage - 1) / problemsPerPage;
            if (page > maxPageCount) return NotFound("I don't have so many problems...");
            return new
            {
                TotalPages = maxPageCount,
                ProblemList = await ProblemModel.GetProblemListAsync((page - 1) * problemsPerPage, problemsPerPage, viewAll),
            };
        }

        [HttpGet]
        [Route("{pid:int}")]
        public async Task<IActionResult> GetProblemAsync(int pid)
        {
            bool viewAll = await UserModel.Authorization.CanViewAllProblemsAsync(HttpContext.Session.GetString("user"));
            if (!viewAll)
                if (await ProblemModel.IsProblemRestrictedAsync(pid))
                    return StatusCode(StatusCodes.Status403Forbidden, "This problem is private now.");
            var problem = await ProblemModel.GetProblemAsync(pid);
            if (problem is null) return NotFound("No such problem.");
            return Ok(problem);
        }

        [RequestSizeLimit(1 << 20)]
        [HttpPost]
        [Route("{pid:int}/Submit")]
        public async Task<IActionResult> SubmitSolutionAsync(int pid, [FromForm] string lang, [FromForm] string code)
        {
            var user = HttpContext.Session.GetString("user");
            if (user is null) return Unauthorized("Not logged in.");
            if (code.Length > maxSolutionSize)
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    $"Solution size should be less than {maxSolutionSize} bytes.");
            if (!SolutionModel.LangMap.ContainsKey(lang)) return BadRequest("No such language.");
            var viewAll = await UserModel.Authorization.CanViewAllProblemsAsync(user);
            if (!viewAll)
                if (await ProblemModel.IsProblemRestrictedAsync(pid))
                    return StatusCode(StatusCodes.Status403Forbidden, "This problem is private now.");
            if (!await ProblemModel.IsProblemExists(pid)) return NotFound("No such problem.");
            var submit_id = await SolutionModel.SubmitProblemAsync(user, pid, SolutionModel.LangMap[lang], code);
            return Ok(submit_id);
        }
    }
}
