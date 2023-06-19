using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.Helpers;

namespace SocialMediaApi.Logic.Services
{
    public class FeedService : IFeedService
    {
        private readonly IUserGroupService _userGroupService;
        private readonly IAuthService _authService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly SocialMediaApiDbContext _dbContext;

        public FeedService(IUserGroupService userGroupService, IAuthService authService, IUserDetailsService userDetailsService, SocialMediaApiDbContext dbContext)
        {
            _userGroupService = userGroupService;
            _authService = authService;
            _userDetailsService = userDetailsService;
            _dbContext = dbContext;
        }

        public async Task<Pagination<PostViewModel>> GetPostsAsync(int page, int limit)
        {
            var groups = await _userGroupService.GetUserGroupsAsync();
            if (!groups.Results.Any())
            {
                return new Pagination<PostViewModel>(new List<PostViewModel>(), 0, 1, 1);
            }
            var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(_authService, _userDetailsService);
            var ids = groups.Results.Select(x => x.Id);
            var posts = await _dbContext.Posts.OrderByDescending(x => x.ActionBasedDate).Skip((page - 1) * limit).Take(limit).Where(x => ids.Contains(x.OwnerId)).ToListAsync();
            var totalItems = (posts.Count == page) ? page + 1 : posts.Count;
            return Pagination<PostViewModel>.GetPagination(posts, totalItems, x => PostMapper.ToView(x, reactions), 1, posts.Count)!;
        }

        public async Task<Pagination<PostViewModel>> GetTrendingPostsAsync(int page, int limit)
        {
            var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(_authService, _userDetailsService);
            var posts = await _dbContext.ActivePosts.OrderByDescending(x => x.Rank).Skip((page - 1) * limit).ToListAsync();
            var totalItems = (posts.Count == page) ? page + 1 : posts.Count;
            return Pagination<PostViewModel>.GetPagination(posts, totalItems, x => PostMapper.ToView(x, reactions), 1, posts.Count)!;
        }
    }
}