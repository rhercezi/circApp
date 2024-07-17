using Circles.Command.Application.DTOs;
using Core.Messages;
using Microsoft.AspNetCore.JsonPatch;

namespace Circles.Command.Application.Commands
{
    public class UpdateCircleCommand : BaseCommand
    {
        public required JsonPatchDocument JsonPatchDocument { get; set; }  
        public Guid CircleId
        {
            get => Id;
            set => Id = value; 
        }
    }
}