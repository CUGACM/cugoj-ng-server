using cugoj_ng_server.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace cugoj_ng_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        [HttpGet]
        [Route("List")]
        public IEnumerable<object> List()
        {
            using var conn = DbConn.GetConnection();
            return conn.Query(@"select user_id,title,content from news where defunct='N' order by news_id desc").Select(x => new
            {
                Author = x.user_id,
                Title = x.title,
                Content = x.content,
                Log = UserModel.Authorization.CanViewAllProblemsAsync("admin")
            });
        }
    }
}
