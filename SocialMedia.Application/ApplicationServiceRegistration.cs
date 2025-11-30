using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Domain.Events;
using SocialMedia.Domain.ReadModels;
using SocialMedia.Application.Features.Posts.EventHandlers;

namespace SocialMedia.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDispatcher, Dispatcher>();

            // Register validators manually
            services.AddScoped<Common.Validation.IValidator<CreateGroupCommand>,
                Features.Groups.Commands.CreateGroup.CreateGroupCommandValidator>();

            // Register Handlers Manually
            services.AddScoped<ICommandHandler<CreatePostCommand, Guid>, CreatePostCommandHandler>();
            services.AddScoped<IQueryHandler<GetPostsQuery, PagedResult<PostDto>>, GetPostsQueryHandler>();
            services.AddScoped<IQueryHandler<GetPostByIdQuery, PostDto?>, GetPostByIdQueryHandler>();

            // Comments
            services.AddScoped<ICommandHandler<CreateCommentCommand, Guid>, CreateCommentCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateCommentCommand, bool>, UpdateCommentCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteCommentCommand, bool>, DeleteCommentCommandHandler>();
            services.AddScoped<IQueryHandler<GetCommentByIdQuery, CommentDto>, GetCommentByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetCommentsByPostIdQuery, PagedResult<CommentDto>>, GetCommentsByPostIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetPostCommentsQuery, PagedResult<CommentReadDto>>, GetPostCommentsQueryHandler>();

            services.AddScoped<ICommandHandler<ToggleLikeCommand, bool>, ToggleLikeCommandHandler>();

            services.AddScoped<ICommandHandler<CreatePollCommand, Guid>, CreatePollCommandHandler>();
            services.AddScoped<ICommandHandler<VoteOnPollCommand, bool>, VoteOnPollCommandHandler>();
            services.AddScoped<IQueryHandler<GetPollQuery, PollDto?>, GetPollQueryHandler>();
            services.AddScoped<IQueryHandler<GetActivePollsQuery, PagedResult<PollDto>>, GetActivePollsQueryHandler>();
            services.AddScoped<IQueryHandler<VerifyChainQuery, bool>, VerifyChainQueryHandler>();

            // Reports
            services.AddScoped<ICommandHandler<ReportPostCommand, Guid>, ReportPostCommandHandler>();
            services.AddScoped<ICommandHandler<ReportCommentCommand, Guid>, ReportCommentCommandHandler>();
            services.AddScoped<IQueryHandler<GetPendingReportsQuery, PagedResult<ReportDto>>, GetPendingReportsQueryHandler>();

            // Auth
            services.AddScoped<ICommandHandler<LoginCommand, AuthResponse>, LoginCommandHandler>();
            services.AddScoped<ICommandHandler<RegisterCommand, AuthResponse>, RegisterCommandHandler>();

            // Register Event Handlers
            services.AddScoped<IEventHandler<PostCreatedEvent>, EmailNotificationHandler>();
            services.AddScoped<IEventHandler<PostCreatedEvent>, PostEventHandlers>();
            services.AddScoped<IEventHandler<LikeAddedEvent>, PostEventHandlers>();
            services.AddScoped<IEventHandler<CommentAddedEvent>, PostEventHandlers>();

            // Groups
            services.AddScoped<ICommandHandler<CreateGroupCommand, Guid>, CreateGroupCommandHandler>();
            services.AddScoped<ICommandHandler<AddUserToGroupCommand, bool>, AddUserToGroupCommandHandler>();
            services.AddScoped<ICommandHandler<RemoveUserFromGroupCommand, bool>, RemoveUserFromGroupCommandHandler>();

            // Users
            services.AddScoped<ICommandHandler<BlockUserCommand, bool>, BlockUserCommandHandler>();
            services.AddScoped<ICommandHandler<AdminBlockUserCommand, bool>, AdminBlockUserCommandHandler>();
            services.AddScoped<IQueryHandler<GetReportedUsersQuery, List<ReportedUserDto>>, GetReportedUsersQueryHandler>();

            // Moderation
            services.AddScoped<ICommandHandler<DeleteReportedContentCommand, int>, DeleteReportedContentCommandHandler>();

            // Notifications
            services.AddScoped<IQueryHandler<GetNotificationsQuery, List<NotificationDto>>, GetNotificationsQueryHandler>();

            return services;
        }
    }
}
