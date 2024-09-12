using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Circles.Query.Application.DTOs
{
    public class JoinRequestDto
    {
        public Guid RequestId { get; set; }
        public Guid CircleId { get; set; }
        public string CircleName { get; set; }
        public string InviterName { get; set; }
        public string InviterSurname { get; set; }
        public string InviterUserName { get; set; }
    }
}