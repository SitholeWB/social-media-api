using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Mappers
{
    public struct UserMapper
    {
        public static UserViewModel? ToView(User? user)
        {
            if (user == null)
            {
                return default;
            }
            return new UserViewModel
            {
                CreatedDate = user.CreatedDate,
                Id = user.Id,
                LastModifiedDate = user.LastModifiedDate,
                AboutMe = user.AboutMe,
                Email = user.Email,
                FirstName = user.FirstName,
                ImageUrl = user.ImageUrl,
                IsApproved = user.IsApproved,
                LastName = user.LastName,
                Password = user.Password,
                UserName = user.UserName,
            };
        }
    }
}