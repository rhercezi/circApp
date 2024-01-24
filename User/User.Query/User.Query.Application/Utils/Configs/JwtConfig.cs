using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Query.Application.Utils.Configs
{
    public class JwtConfig
    {
        public required string Secret { get; set; }
    }
}