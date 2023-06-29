using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Messages
{
    public abstract class BaseCommand
    {
        public Guid Id { get; set; }
    }
}