using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly UserRepository _userRepository;

        public GetUserByEmailQueryHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> HandleAsync(GetUserByEmailQuery query)
        {
            var user = await _userRepository.GetUserByEmailAsync(query.Email);
            return new UserDto
            {
                Id = user.Id,
                Created = user.Created,
                Updated = user.Updated,
                UserName = user.UserName,
                FirstName = user.FirstName,
                FamilyName = user.FamilyName,
                Email = user.Email,
                EmailVerified = user.EmailVerified
            };
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((GetUserByEmailQuery)query);
        }
    }
}