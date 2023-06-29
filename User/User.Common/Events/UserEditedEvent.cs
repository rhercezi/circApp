using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace User.Common.Events
{
    public record UserEditedEvent(Guid Id,
                                  int Version,
                                  string UserName,
                                  string Password,
                                  string FirstName,
                                  string FamilyName,
                                  string Email,
                                  bool EmailConfirmed,
                                  DateTime Changed) : BaseEvent(Id, Version, typeof(UserEditedEvent));
}