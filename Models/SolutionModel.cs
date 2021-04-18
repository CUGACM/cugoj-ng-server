using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cugoj_ng_server.Utilities;
using Dapper;

namespace cugoj_ng_server.Models
{
    public class SolutionModel : DbConn
    {
        public enum Language
        {
            C,
            CPP,
            Java,
            Python
        }
        public static readonly Dictionary<string, Language> LangMap = new();
        static SolutionModel()
        {
            foreach (var lang in Enum.GetValues<Language>())
                LangMap.Add(Enum.GetName(lang), lang);
        }

        public static async Task<int> SubmitProblemAsync(string user, int pid, Language lang, string code)
        {
            using var conn = GetConnection();
            var langid = HUSTOJ.MapLanguageToId(Enum.GetName(lang));
            var submit_id = await conn.QueryFirstAsync<int>(@"INSERT INTO solution(problem_id,user_id,in_date,language,ip,code_length,result) 
                VALUES(@pid,@uid,now(),@lang,'0.0.0.0',@len,@status);
                SELECT LAST_INSERT_ID();", new
            {
                pid,
                uid = user,
                lang = langid,
                len = code.Length,
                status = HUSTOJ.Status.MSG_Other
            });
            var insert_1 = conn.ExecuteAsync(@"INSERT INTO `source_code`(`solution_id`,`source`) VALUES (@sid,@code)", new { sid = submit_id, code });
            // I don't know why there's 2 tables, just make hustoj happy...
            var insert_2 = conn.ExecuteAsync(@"INSERT INTO `source_code_user`(`solution_id`,`source`) VALUES (@sid,@code)", new { sid = submit_id, code });
            await Task.WhenAll(insert_1, insert_2);
            await conn.ExecuteAsync(@"UPDATE solution SET result=0 WHERE solution_id=@sid", new { sid = submit_id });
            return submit_id;
        }
    }
}
