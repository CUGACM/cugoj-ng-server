using cugoj_ng_server.Models;
using cugoj_ng_server.Utilities;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cugoj_ng_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync([FromForm] string username, [FromForm] string password)
        {
            var curTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var lastTry = HttpContext.Session.Get("LastTryLogin")?.Decode<long>() ?? 0;
            HttpContext.Session.Set("LastTryLogin", curTimestamp.Encode());
            if (curTimestamp - lastTry < 5)
                return StatusCode(StatusCodes.Status429TooManyRequests, "Too Many Requests, wait for 5 seconds.");
            var res = await UserModel.Authentication.LoginAsync(username, password);
            switch (res)
            {
                case UserModel.Authentication.LoginResult.Success:
                    HttpContext.Session.SetString("user", username);
                    return Ok("Logged in");
                case UserModel.Authentication.LoginResult.NotExist:
                    return Unauthorized("User not exist");
                case UserModel.Authentication.LoginResult.WrongPassword:
                    return Unauthorized("Password not correct");
                case UserModel.Authentication.LoginResult.Banned:
                    return StatusCode(StatusCodes.Status403Forbidden, "You are banned");
            }
            return BadRequest();
        }

        [Route("Logout")]
        public void Logout() => HttpContext.Session.Clear();

        [Route("WhoAmI")]
        public object WhoAmI()
        {
            var user = HttpContext.Session.GetString("user");
            if (user is null) return new { user };
            return new
            {
                user,
                privileges = UserModel.Authorization.GetPrivilegesAsync(user)
            };
        }
    }
}
