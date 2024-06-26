﻿using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Domain.Models.UserPosts;
using SubPub.Hangfire;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Posts
{
    public class AddPostNotificationHandler : IHangfireEventHandler<AddPostEvent>
    {
        private readonly IActivePostService _newPostService;
        private readonly IUserPostService _userPostService;

        public AddPostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
            _userPostService = postUnitOfWork.UserPostService;
        }

        public async Task RunAsync(AddPostEvent obj)
        {
            if (obj?.Post != null)
            {
                await _userPostService.AddUserPostAsync(new AddUserPostModel
                {
                    CreatedDate = obj.Post!.CreatedDate,
                    EntityId = obj.Post!.Id,
                    UserId = obj.Post!.Creator.Id,
                });
                await _newPostService.AddActivePostAsync(obj.Post!.OwnerId, new AddActivePostModel
                {
                    Post = obj.Post
                });
            }
        }
    }
}