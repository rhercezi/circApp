using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.MessageHandling;
using User.Common.Events;

namespace User.Command.Domain.Aggregates.AggregateActions
{
    public class CreateUserAction : IAggregateAction<UserCreatedEvent, UserAggreate>
    {
        public void ExecuteAsync(UserCreatedEvent xEvent, UserAggreate instance)
        {
            ValidateEvent(xEvent);
            instance._id = xEvent.Id;
            instance._version = xEvent.Version;
            instance._events.Add(xEvent);
        }

        private void ValidateEvent(UserCreatedEvent xEvent)
        {
            var validUserName = new Regex(@"^(?=[a-zA-Z])[-\w.]{1,16}([a-zA-Z\d]$");
            var mustContain = "A valid username can contain only letters numbers or underscore and can not be longer than 16 characters";
            if (!validUserName.IsMatch(xEvent.UserName)) throw new FormatException($"Username {xEvent.UserName} is not a valid Username. " + mustContain);

            var validName = new Regex(@"^(?=[a-zA-Z]){1,16}");
            mustContain = "Only letters are allowed and length of maximum 16 characters";
            if (!validUserName.IsMatch(xEvent.FirstName)) throw new FormatException($"{xEvent.FirstName} is not a valid name. " + mustContain);
            if (!validUserName.IsMatch(xEvent.FamilyName)) throw new FormatException($"{xEvent.FamilyName} is not a valid last name. " + mustContain);

            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var validEmail = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!validEmail.IsMatch(xEvent.Email)) throw new FormatException($"Email addtess {xEvent.Email} is not a valid email address");

            pattern = @"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$";
            var validPassword = new Regex(pattern);
            mustContain = "Password must contain at least one lower case letter, at least one upper case letter, "
            + "at least special character, at least one number, at least 8 characters length";
            if (!validEmail.IsMatch(xEvent.Password)) throw new FormatException($"Password is invalid. " + mustContain);

        }
    }
}