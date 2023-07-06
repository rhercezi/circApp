using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Configs
{
    public class MongoDbConfig
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string CollectionName { get; set; }
    }
}