using System.Text.RegularExpressions;
using Core.Messages;
using User.Command.Api.Commands;
using User.Command.Application.Commands;
using User.Command.Application.Exceptions;
using User.Command.Domin.Stores;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Validation
{
    public abstract class UserCommandValidatorBase<T>
    {
        protected readonly EventStore _eventStore;

        protected UserCommandValidatorBase(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public abstract Task ValidateCommand(T command);
        protected string ValidateNameString(string name, string nameType)
        {
            var regex = new Regex(@"^(?=[a-zA-Z])[-\w.]{1,16}([a-zA-Z\d]$)");
            var mustContain = $"A valid {nameType} can contain only letters numbers or underscore and can not be longer than 16 characters";
            if (!regex.IsMatch(name))
            {
                return $"{name} is not a valid {nameType}. " + mustContain;
            }
            return string.Empty;
        }

        protected string ValidateEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!regex.IsMatch(email))
            {
                return $"Email addtess {email} is not a valid email address";
            }
            return string.Empty;
        }

        protected string ValidatePassword(string password)
        {
            string pattern = @"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$";
            var regex = new Regex(pattern);
            if (!regex.IsMatch(password))
            {
                return $"Password is invalid. " 
                + "Password must contain at least one lower case letter, one upper case letter, "
                + "one special character, one number and must have length of at least 8 characters";
            }
            return string.Empty;
        }

        protected void ThrowIfErrorExists(List<string> messages)
        {
            if (messages.Any())
            {
                string errorMessage = "";

                foreach (var message in messages)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        errorMessage += message + Environment.NewLine;
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    throw new UserValidationException(errorMessage);
                }
            }
        }
        protected async Task<string> ValidateUserExistsByUsername(string userName)
        {
            if (await _eventStore.UsernameExistsAsync(userName))
            {
                return $"User with username {userName} already exists";
            }
            return string.Empty;
        }

        protected async Task<string> ValidateUserExistsByEmail(string email)
        {
            if (await _eventStore.EmailExistsAsync(email))
            {
                return $"User with Email {email} already exists";
            }
            return string.Empty;
        }

        protected string ValidateOldPassword(BaseCommand command, List<Core.Messages.BaseEvent> events, PasswordHashService passwordHashService)
        {
            var lastEvent = events.OrderByDescending(x => x.Version)
                            .ToList()
                            .Find(x => x.GetType() == typeof(UserCreatedEvent) || x.GetType() == typeof(PasswordUpdatedEvent));
            if (lastEvent == null)
            {
                return "Error validating password";
            }
            string oldPassword = string.Empty;
            if (lastEvent.GetType() == typeof(UserCreatedEvent))
            {
                var xEvent = (UserCreatedEvent)lastEvent;
                oldPassword = xEvent.Password;
            }
            else if (lastEvent.GetType() == typeof(PasswordUpdatedEvent))
            {
                var xEvent = (PasswordUpdatedEvent)lastEvent;
                oldPassword = xEvent.Password;
            }

            if (command.GetType() == typeof(UpdatePasswordCommand))
            {
                var castCommand = (UpdatePasswordCommand)command;
                if (!passwordHashService.VerifyPassword(castCommand.OldPassword, oldPassword, castCommand.Id)) return "Wrong password provided";
            }
            else if (command.GetType() == typeof(DeleteUserCommand))
            {
                var castCommand = (DeleteUserCommand)command;
                if (!passwordHashService.VerifyPassword(castCommand.Password, oldPassword, castCommand.Id)) return "Wrong password provided";
            }

            
            return string.Empty;
        }
    }
}