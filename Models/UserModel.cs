using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        public static async Task<ApplicationForm> GetApplicationFormAsync(string uid)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync<ApplicationForm>("select * from application_form where user_id=@uid", new { uid });
        }

        public static async Task<int> SetApplicationFormAsync(ApplicationForm form)
        {
            using var conn = GetConnection();
            return await conn.ExecuteAsync(@"
                REPLACE INTO application_form (user_id,name,student_id,college,major,mobile,qq,score,oj_accounts,text1,text2,text3,text4,status,comment)
                VALUES (@UserId,@Name,@StudentId,@College,@Major,@Mobile,@QQ,@Score,@OJAccounts,@Text1,@Text2,@Text3,@Text4,@Status,@Comment)
            ", form);
        }
    }

    public class ApplicationForm
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string StudentId { get; set; }

        public string College { get; set; }

        public string Major { get; set; }

        public string Mobile { get; set; }

        public string QQ { get; set; }

        public string Score { get; set; }

        [Labels("Nullable")]
        public string OJAccounts { get; set; }

        public string Text1 { get; set; }

        public string Text2 { get; set; }

        public string Text3 { get; set; }

        [Labels("Nullable")]
        public string Text4 { get; set; }

        public string Status { get; set; }

        [Labels("Nullable")]
        public string Comment { get; set; }

        static readonly PropertyInfo[] properties =
            typeof(ApplicationForm).GetProperties().Where(prop => !prop.HasLabel("Nullable")).ToArray();
        
        public bool Validate() => 
            properties.All(prop => 
                !string.IsNullOrWhiteSpace((string)prop.GetValue(this)));
    }
}
