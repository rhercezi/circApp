using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;

namespace User.Query.Application.DTOs
{
    public class RefreshDto : BaseDto
    {
        public TokensDto? Tokens { get; set; }
        public string? Exp { get; set; }
    }
}