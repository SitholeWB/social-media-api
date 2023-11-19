// Ignore Spelling: auth

using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.Models.Security;

namespace SocialMediaApi.Domain.Logic.Services
{
	public class UserDetailsService : IUserDetailsService
	{
		private readonly SocialMediaApiDbContext _dbContext;
		private readonly IAuthService _authService;

		public UserDetailsService(SocialMediaApiDbContext dbContext, IAuthService authService)
		{
			_dbContext = dbContext;
			_authService = authService;
		}

		public async Task AddCommentReactionAsync(AuthUser authUser, Guid entityId, AddEntityReactionModel model)
		{
			if (string.IsNullOrEmpty(model?.Unicode))
			{
				throw new SocialMediaException("Unicode is required.");
			}
			var userDetails = await _dbContext.UserDetails.FindAsync(authUser.AuthorizedUser.Id);
			if (userDetails == null)
			{
				userDetails = new UserDetails
				{
					Id = authUser.AuthorizedUser.Id,
					CreatedDate = DateTimeOffset.UtcNow,
					LastModifiedDate = DateTimeOffset.UtcNow,
					CommentReactions = new List<MiniReaction>(),
					PostReactions = new List<MiniReaction>()
				};
				userDetails.CommentReactions.Add(new MiniReaction
				{
					EntityId = entityId,
					Unicode = model.Unicode,
				});
				await _dbContext.SaveChangesAsync();
			}
			else
			{
				var reaction = userDetails.CommentReactions.FirstOrDefault(x => x.EntityId == entityId);
				userDetails.CommentReactions = userDetails.CommentReactions.Where(x => x.EntityId != entityId).ToList();
				if (reaction == null)
				{
					reaction = new MiniReaction
					{
						EntityId = entityId,
						Unicode = model.Unicode
					};
				}
				else
				{
					reaction.Unicode = model.Unicode;
				}
				userDetails.CommentReactions.Add(reaction);
				_dbContext.Update(userDetails);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task AddPostReactionAsync(AuthUser authUser, Guid entityId, AddEntityReactionModel model)
		{
			if (string.IsNullOrEmpty(model?.Unicode))
			{
				throw new SocialMediaException("Unicode is required.");
			}
			var userDetails = await _dbContext.UserDetails.FindAsync(authUser.AuthorizedUser.Id);
			if (userDetails == null)
			{
				userDetails = new UserDetails
				{
					Id = authUser.AuthorizedUser.Id,
					CreatedDate = DateTimeOffset.UtcNow,
					LastModifiedDate = DateTimeOffset.UtcNow,
					CommentReactions = new List<MiniReaction>(),
					PostReactions = new List<MiniReaction>()
				};
				userDetails.PostReactions.Add(new MiniReaction
				{
					EntityId = entityId,
					Unicode = model.Unicode,
				});
				_dbContext.Add(userDetails);
				await _dbContext.SaveChangesAsync();
			}
			else
			{
				var reaction = userDetails.PostReactions.FirstOrDefault(x => x.EntityId == entityId);
				userDetails.PostReactions = userDetails.PostReactions.Where(x => x.EntityId != entityId).ToList();
				if (reaction == null)
				{
					reaction = new MiniReaction
					{
						EntityId = entityId,
						Unicode = model.Unicode
					};
				}
				else
				{
					reaction.Unicode = model.Unicode;
				}
				userDetails.PostReactions.Add(reaction);
				_dbContext.Update(userDetails);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task DeleteCommentReactionAsync(AuthUser authUser, Guid entityId)
		{
			var userDetails = await _dbContext.UserDetails.FindAsync(authUser.AuthorizedUser.Id);
			if (userDetails != null)
			{
				userDetails.CommentReactions = userDetails.CommentReactions.Where(x => x.EntityId != entityId).ToList();
				_dbContext.Update(userDetails);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task DeletePostReactionAsync(AuthUser authUser, Guid entityId)
		{
			var userDetails = await _dbContext.UserDetails.FindAsync(authUser.AuthorizedUser.Id);
			if (userDetails != null)
			{
				userDetails.PostReactions = userDetails.PostReactions.Where(x => x.EntityId != entityId).ToList();
				_dbContext.Update(userDetails);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task<IList<MiniReaction>> GetCommentReactionsAsync(AuthUser authUser)
		{
			return await _dbContext.UserDetails.AsNoTracking().Where(x => x.Id == authUser.AuthorizedUser.Id).Select(x => x.CommentReactions).FirstOrDefaultAsync() ?? new List<MiniReaction>();
		}

		public async Task<IList<MiniReaction>> GetPostReactionsAsync(AuthUser authUser)
		{
			return await _dbContext.UserDetails.AsNoTracking().Where(x => x.Id == authUser.AuthorizedUser.Id).Select(x => x.PostReactions).FirstOrDefaultAsync() ?? new List<MiniReaction>();
		}
	}
}