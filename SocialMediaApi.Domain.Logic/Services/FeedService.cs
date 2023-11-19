using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Logic.Helpers;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Logic.Services
{
	public class FeedService : IFeedService
	{
		private readonly IUserGroupService _userGroupService;
		private readonly IUserDetailsService _userDetailsService;
		private readonly SocialMediaApiDbContext _dbContext;

		public FeedService(IUserGroupService userGroupService, IUserDetailsService userDetailsService, SocialMediaApiDbContext dbContext)
		{
			_userGroupService = userGroupService;
			_userDetailsService = userDetailsService;
			_dbContext = dbContext;
		}

		public async Task<Pagination<PostViewModel>> GetAnonymousPostsAsync(AuthUser authUser, int page, int limit, bool skipActivePosts)
		{
			var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(authUser, _userDetailsService);
			if (!skipActivePosts)
			{
				var activePosts = await _dbContext.ActivePosts.OrderByDescending(x => x.ActionBasedDate).Skip((page - 1) * limit).Take(limit).ToListAsync();
				if (activePosts.Count >= (limit / 2))
				{
					return Pagination<PostViewModel>.GetPagination(activePosts, limit + 1, x => PostMapper.ToView(x, reactions), 1, activePosts.Count)!;
				}
			}

			var posts = await _dbContext.Posts.OrderByDescending(x => x.ActionBasedDate).Skip((page - 1) * limit).Take(limit).ToListAsync();
			var totalItems = (posts.Count == page) ? page + 1 : posts.Count;
			return Pagination<PostViewModel>.GetPagination(posts, totalItems, x => PostMapper.ToView(x, reactions), 1, posts.Count)!;
		}

		public async Task<Pagination<PostViewModel>> GetPostsAsync(AuthUser authUser, int page, int limit, bool skipActivePosts)
		{
			if (!authUser.IsAuthenticated)
			{
				return await GetAnonymousPostsAsync(authUser, page, limit, skipActivePosts);
			}
			var groups = await _userGroupService.GetUserGroupsAsync(authUser);
			if (!groups.Results.Any())
			{
				return await GetAnonymousPostsAsync(authUser, page, limit, skipActivePosts);
			}
			var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(authUser, _userDetailsService);
			var ids = groups.Results.Select(x => x.Id);
			if (!skipActivePosts)
			{
				var activePosts = await _dbContext.ActivePosts.OrderByDescending(x => x.ActionBasedDate).Skip((page - 1) * limit).Take(limit).Where(x => ids.Contains(x.OwnerId)).ToListAsync();
				if (activePosts.Count >= (limit / 2))
				{
					return Pagination<PostViewModel>.GetPagination(activePosts, limit + 1, x => PostMapper.ToView(x, reactions), 1, activePosts.Count)!;
				}
			}

			var posts = await _dbContext.Posts.OrderByDescending(x => x.ActionBasedDate).Skip((page - 1) * limit).Take(limit).Where(x => ids.Contains(x.OwnerId)).ToListAsync();
			if (posts.Count < (limit / 4))
			{
				//Too little post found for user, add Anonymous Posts
				var anonymousPosts = await _dbContext.Posts.OrderByDescending(x => x.ActionBasedDate).Skip((page - 1) * limit).Take(limit).ToListAsync();
				posts.AddRange(anonymousPosts);
			}

			var totalItems = (posts.Count == page) ? page + 1 : posts.Count;
			return Pagination<PostViewModel>.GetPagination(posts, totalItems, x => PostMapper.ToView(x, reactions), 1, posts.Count)!;
		}

		public async Task<Pagination<PostViewModel>> GetTrendingPostsAsync(AuthUser authUser, int page, int limit)
		{
			var date = DateTimeOffset.UtcNow.AddDays(-2);
			var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(authUser, _userDetailsService);
			var posts = await _dbContext.Posts.Where(x => x.ActionBasedDate > date).OrderByDescending(x => x.Rank).Skip((page - 1) * limit).ToListAsync();
			var totalItems = (posts.Count == page) ? page + 1 : posts.Count;
			return Pagination<PostViewModel>.GetPagination(posts, totalItems, x => PostMapper.ToView(x, reactions), 1, posts.Count)!;
		}
	}
}