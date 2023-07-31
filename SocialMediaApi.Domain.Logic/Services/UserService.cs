using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Common;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Users;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Domain.Interfaces;

namespace SocialMediaApi.Domain.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly SocialMediaApiDbContext _dbContext;

        public UserService(SocialMediaApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserViewModel> AddUserAsync(AddUserModel model)
        {
            if (string.IsNullOrEmpty(model?.UserName))
            {
                throw new SocialMediaException("UserName is required.");
            }
            if (string.IsNullOrEmpty(model?.FirstName))
            {
                throw new SocialMediaException("FirstName is required.");
            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
            if (user != null)
            {
                throw new SocialMediaException($"User name {model.UserName} already registered.");
            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user != null)
                {
                    throw new SocialMediaException($"Email {model.Email} already registered.");
                }
            }
            var userId = Guid.NewGuid();
            user = new User
            {
                UserName = model.UserName,
                AboutMe = model.UserName,
                Email = model.Email,
                CreatedDate = DateTimeOffset.UtcNow,
                FirstName = model.FirstName,
                Id = userId,
                ImageUrl = model.ImageUrl,
                IsApproved = false,
                LastModifiedDate = DateTimeOffset.UtcNow,
                LastName = model.LastName,
                Password = HashingUtils.HashUserPassword(userId, model.Password),
            };
            var addedEntity = _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
            return UserMapper.ToView(addedEntity.Entity)!;
        }

        public async Task ApproveUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id) ?? throw new SocialMediaException($"User for given Id not found.");
            user.IsApproved = true;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id) ?? throw new SocialMediaException($"User for given Id not found.");

            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserViewModel?> GetUserAsync(Guid id)
        {
            return UserMapper.ToView(await _dbContext.Users.FindAsync(id));
        }

        public async Task<UserViewModel?> GetUserByEmailAsync(string email)
        {
            return UserMapper.ToView(await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email.Trim()));
        }

        public async Task<UserViewModel?> GetUserByUserNameAsync(string userName)
        {
            return UserMapper.ToView(await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName.Trim()));
        }

        public async Task<Pagination<UserViewModel>> GetUsersAsync(int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<User, UserViewModel>(page, limit, UserMapper.ToView!);
        }

        public async Task<UserViewModel> UpdateUserAsync(Guid id, UpdateUserModel model)
        {
            if (string.IsNullOrEmpty(model?.FirstName))
            {
                throw new SocialMediaException("FirstName is required.");
            }
            var user = await _dbContext.Users.FindAsync(id) ?? throw new SocialMediaException($"User for given Id not found.");
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.AboutMe = model.AboutMe;
            user.ImageUrl = model.ImageUrl;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            return UserMapper.ToView(user)!;
        }

        public async Task UpdateUserPasswordAsync(Guid id, UpdateUserPasswordModel model)
        {
            if (string.IsNullOrEmpty(model?.Password))
            {
                throw new SocialMediaException("Password is required.");
            }
            var user = await _dbContext.Users.FindAsync(id) ?? throw new SocialMediaException($"User for given Id not found.");
            user.Password = HashingUtils.HashUserPassword(id, model.Password);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}