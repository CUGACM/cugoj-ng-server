using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace cugoj_ng_server.Models
{
    public class DbConn
    {
        public static Func<IDbConnection> GetConnection { get; private set; }
        public static IConnectionMultiplexer ConnectionMultiplexer { get; private set; }
    }
}
