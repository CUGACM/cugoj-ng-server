using cugoj_ng_server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cugoj_ng_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationFormController : ControllerBase
    {
        [Route("Get"), HttpGet]
        public async Task<ActionResult<ApplicationForm>> GetAsync()
        {
            var user = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(user))
                return Unauthorized();
            var form = await UserModel.GetApplicationFormAsync(user);
            if (form == null) return NoContent();
            return Ok(form);
        }
        [Route("Save"), HttpPost]
        public async Task<IActionResult> SaveAsync([FromBody] ApplicationForm form)
        {
            var user = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(user))
                return Unauthorized();
            form.UserId = user;
            return Ok(await UserModel.SetApplicationFormAsync(form));
        }

        [Route("Submit"), HttpPost]
        public async Task<IActionResult> SubmitAsync([FromBody] ApplicationForm form)
        {
            var user = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(user))
                return Unauthorized();
            form.UserId = user;
            if (!form.Validate())
                return BadRequest();
            return Ok(await UserModel.SetApplicationFormAsync(form));
        }
    }
}
