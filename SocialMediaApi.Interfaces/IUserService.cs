﻿using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Users;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IUserService
    {
        public Task<UserViewModel> AddUserAsync(AddUserModel model);

        public Task<UserViewModel> UpdateUserAsync(Guid id, UpdateUserModel model);

        public Task ApproveUserAsync(Guid id);

        public Task UpdateUserPasswordAsync(Guid id, UpdateUserPasswordModel model);

        public Task DeleteUserAsync(Guid id);

        public Task<Pagination<UserViewModel>> GetUsersAsync(int page = 1, int limit = 20);
    }
}