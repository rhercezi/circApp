using Core.Configs;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tasks.Domain.Entities;
using Tasks.Domain.Repositories;
using Tasks.Query.Application.Config;
using Tasks.Query.Application.Queries;

namespace Tasks.Query.Application.Handlers
{
    public class GetTasksForUserQueryHandler : IMessageHandler<GetTasksForUserQuery>
    {
        private readonly AppTaskRepository _taskRepository;
        private readonly InternalHttpClient<AppUserDto> _internalHttp;
        private readonly IOptions<CirclesServiceConfig> _config;
        private readonly ILogger<GetTasksForUserQueryHandler> _logger;

        public GetTasksForUserQueryHandler(AppTaskRepository taskRepository,
                                           InternalHttpClient<AppUserDto> internalHttp,
                                           IOptions<CirclesServiceConfig> config,
                                           ILogger<GetTasksForUserQueryHandler> logger)
        {
            _taskRepository = taskRepository;
            _internalHttp = internalHttp;
            _config = config;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(GetTasksForUserQuery query)
        {
            try
            {
                var clientConfig = new HttpClientConfig
                {
                    BaseUrl = _config.Value.BaseUrl,
                    Path = _config.Value.Path + query.UserId.ToString(),
                    Port = _config.Value.Port
                };
                _logger.LogDebug("http client config: {@Config}", clientConfig);
    
                var tasks = new List<AppTaskModel>();

                if (query.SearchByCircles)
                {
                    _logger.LogDebug("Searching by circles");
                    var appUserDto = await _internalHttp.GetResource(clientConfig);
                    _logger.LogDebug("User data: {@User}", appUserDto);
                    if (appUserDto.Circles != null && appUserDto.Circles.Count > 0)
                    {
                        tasks = await _taskRepository.GetTasksByCircleIds(appUserDto.Circles.Select(c => c.Id).ToList(), query.IncludeCompleted);
                        _logger.LogDebug("Found {Nr} tasks for circles: {Circles}", tasks.Count, appUserDto.Circles.Select(c => c.Id).ToList());
                    }
                }
                else
                {
                    _logger.LogDebug("Searching by user id");
                    tasks = await _taskRepository.GetTasksByUserId(query.UserId, query.IncludeCompleted);
                    _logger.LogDebug("Found {Nr} tasks for user id {Id}", tasks.Count, query.UserId);
                }

                return  new BaseResponse { ResponseCode = 200, Data = tasks };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((GetTasksForUserQuery)query);
        }
    }
}