namespace SocialMedia.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        // Register validators manually
        services.AddScoped<IValidator<CreateGroupCommand>, CreateGroupCommandValidator>();

        // Register Handlers Manually
        services.AddScoped<ICommandHandler<CreatePostCommand, Guid>, CreatePostCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePostCommand, bool>, DeletePostCommandHandler>();
        services.AddScoped<IQueryHandler<GetPostsQuery, PagedResult<PostDto>>, GetPostsQueryHandler>();
        services.AddScoped<IQueryHandler<GetPostByIdQuery, PostDto?>, GetPostByIdQueryHandler>();

        // Comments
        services.AddScoped<ICommandHandler<CreateCommentCommand, Guid>, CreateCommentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCommentCommand, bool>, UpdateCommentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCommentCommand, bool>, DeleteCommentCommandHandler>();
        services.AddScoped<IQueryHandler<GetCommentByIdQuery, CommentReadDto>, GetCommentByIdQueryHandler>();
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
        services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedEventHandler>();
        services.AddScoped<IEventHandler<PostLikeAddedEvent>, PostLikedAddedEventHandler>();
        services.AddScoped<IEventHandler<PostDeletedEvent>, PostDeletedEventHandler>();
        services.AddScoped<IEventHandler<CommentAddedEvent>, CommentAddedEventHandler>();
        services.AddScoped<IEventHandler<CommentLikeAddedEvent>, CommentLikeAddedEventHandler>();
        services.AddScoped<IEventHandler<CommentDeletedEvent>, CommentDeletedEventHandler>();
        services.AddScoped<IEventHandler<PostLikeAddedEvent>, UserActivityEventHandler>();
        services.AddScoped<IEventHandler<CommentLikeAddedEvent>, UserActivityEventHandler>();
        services.AddScoped<IEventHandler<PollVotedEvent>, UserActivityEventHandler>();

        // Groups
        services.AddScoped<ICommandHandler<CreateGroupCommand, Guid>, CreateGroupCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteGroupCommand, bool>, DeleteGroupCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateGroupCommand, bool>, UpdateGroupCommandHandler>();
        services.AddScoped<ICommandHandler<AddUserToGroupCommand, bool>, AddUserToGroupCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveUserFromGroupCommand, bool>, RemoveUserFromGroupCommandHandler>();
        services.AddScoped<ICommandHandler<CreateDefaultGroupsCommand, string>, CreateDefaultGroupsCommandHandler>();
        services.AddScoped<IQueryHandler<GetGroupsQuery, PagedResult<GroupDto>>, GetGroupsQueryHandler>();
        services.AddScoped<IQueryHandler<GetGroupQuery, GroupDto?>, GetGroupQueryHandler>();

        // Users
        services.AddScoped<ICommandHandler<BlockUserCommand, bool>, BlockUserCommandHandler>();
        services.AddScoped<ICommandHandler<AdminBlockUserCommand, bool>, AdminBlockUserCommandHandler>();
        services.AddScoped<IQueryHandler<GetReportedUsersQuery, List<ReportedUserDto>>, GetReportedUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, AuthResponse>, GetUserByIdQueryHandler>();

        // Moderation
        services.AddScoped<ICommandHandler<DeleteReportedContentCommand, int>, DeleteReportedContentCommandHandler>();

        // polls
        services.AddScoped<ICommandHandler<DeletePollCommand, bool>, DeletePollCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePollCommand, bool>, UpdatePollCommandHandler>();

        // stats
        services.AddScoped<IQueryHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>, GetDashboardStatsQueryHandler>();

        // Notifications
        services.AddScoped<IQueryHandler<GetNotificationsQuery, List<NotificationDto>>, GetNotificationsQueryHandler>();

        return services;
    }
}