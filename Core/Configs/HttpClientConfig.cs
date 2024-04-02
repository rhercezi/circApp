using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Configs
{
    public class HttpClientConfig
    {
        public required string BaseUrl { get; set; }
        public required string Path { get; set; }
        public required int Port { get; set; }
    }
}