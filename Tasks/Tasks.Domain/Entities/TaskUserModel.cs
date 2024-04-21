using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tasks.Domain.Entities
{
    public class TaskUserModel
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}