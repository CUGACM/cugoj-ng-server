using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cugoj_ng_server.Utilities
{
    using static Crypto;
    using static Sequence;
    public static class HUSTOJ
    {
        static readonly Dictionary<string, int> langMap = new();
        public enum Langs
        {
            C, CPP, Pascal, Java, Ruby, Bash, Python, PHP, Perl, CSharp, ObjC, FreeBasic, Scheme, Clang, ClangPP, Lua, JavaScript, Go, Other
        };
        public enum Status
        {
            MSG_Pending,
            MSG_Pending_Rejudging,
            MSG_Compiling,
            MSG_Running_Judging,
            MSG_Accepted,
            MSG_Presentation_Error,
            MSG_Wrong_Answer,
            MSG_Time_Limit_Exceed,
            MSG_Memory_Limit_Exceed,
            MSG_Output_Limit_Exceed,
            MSG_Runtime_Error,
            MSG_Compile_Error,
            MSG_Compile_OK,
            MSG_TEST_RUN,
            MSG_Other
        }
        static HUSTOJ()
        {
            foreach (var lang in Enum.GetValues<Langs>())
                langMap.Add(Enum.GetName(lang), (int)lang);
        }
        public static bool CheckPw(string password, string saved)
        {
            /*
            function pwCheck($password,$saved)
            {
              $svd=base64_decode($saved);
              $salt=substr($svd,20);
              if(!isOldPW($password)) $password=md5($password);
              $hash = base64_encode( sha1(($password) . $salt, true) . $salt );
              if (strcmp($hash,$saved)==0) return True;
              else return False;
            }
             */
            ReadOnlySpan<byte> pswdBytes = saved.Base64Decode();
            if (pswdBytes.Length != 24) return false;
            ReadOnlySpan<byte> finalHash = SHA1(ByteArrayConcat(MD5(password.Encode()).HexEncode().Encode(), pswdBytes[20..].ToArray()));
            if (!pswdBytes[..20].SequenceEqual(finalHash)) return false;
            return true;
        }
        public static int MapLanguageToId(string lang) => langMap[lang];
    }
}
