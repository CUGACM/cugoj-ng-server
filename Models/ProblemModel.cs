using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace cugoj_ng_server.Models
{
    public class ProblemModel : DbConn
    {
        const string privateProblemsFilter = "where problem_id not in (SELECT problem_id from private_problems)";
        public static async Task<int> GetProblemsCountAsync(bool viewAll = false)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(problem_id) from problem " + (viewAll ? "" : privateProblemsFilter));
        }
        public static async Task<IEnumerable<dynamic>> GetProblemListAsync(int skip, int limit, bool viewAll = false)
        {
            using var conn = GetConnection();
            return await conn.QueryAsync(
                @"SELECT problem_id,title,source,submit,accepted from problem " +
                (viewAll ? "" : privateProblemsFilter) + @"limit @skip,@limit",
                new { skip, limit }
            );
        }
        public static async Task<dynamic> GetProblemAsync(int pid)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync(@"SELECT * FROM problem where problem_id=@pid", new { pid });
        }
        public static async Task<bool> IsProblemRestrictedAsync(int pid)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync<int>(@"select count(problem_id) from private_problems where problem_id=@id", new { id = pid }) > 0;
        }
        public static async Task<bool> IsProblemExists(int pid)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync<int>(@"select count(problem_id) from problem where problem_id=@id", new { id = pid }) > 0;
        }
    }
}
