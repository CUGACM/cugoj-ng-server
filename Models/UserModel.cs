using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using cugoj_ng_server.Utilities;
using Dapper;

namespace cugoj_ng_server.Models
{
    public class UserModel : DbConn
    {
        public class Authentication
        {
            public enum LoginResult
            {
                Success,
                NotExist,
                WrongPassword,
                Banned,
            }
            public static async Task<LoginResult> LoginAsync(string username, string password)
            {
                using var conn = GetConnection();
                var res = await conn.QueryFirstOrDefaultAsync<(
                    string user,
                    string password,
                    string defunct
                    )>(@"select user_id,password,defunct from users where user_id=@id", new { id = username });
                if (res.user == null)
                    return LoginResult.NotExist;
                if (res.defunct == "Y")
                    return LoginResult.Banned;
                if (!HUSTOJ.CheckPw(password, res.password))
                    return LoginResult.WrongPassword;
                return LoginResult.Success;
            }
        }
        public class Authorization
        {
            public static async Task<bool> CanViewAllProblemsAsync(string user_id)
            {
                if (user_id == null) return false;
                using var conn = GetConnection();
                return await conn.QueryFirstOrDefaultAsync<int>(
                    @"SELECT count(DISTINCT rightstr) FROM privilege 
                    where user_id=@id and rightstr in ('administrator','contest_creator')",
                    new { id = user_id }) > 0;
            }
            public static async Task<string[]> GetPrivilegesAsync(string user_id)
            {
                if (string.IsNullOrEmpty(user_id)) return null;
                using var conn = GetConnection();
                return (await conn.QueryAsync<string>(@"select rightstr from privilege where user_id=@id and length(rightstr)>5", new { id = user_id })).ToArray();
            }
        }
    }
}
