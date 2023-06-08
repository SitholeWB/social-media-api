﻿using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.ViewModels
{
    public class GroupPostViewModel : BasePost
    {
        public Guid GroupId { get; set; }
        public EntityOrigin EntityOrigin { get; set; }
    }
}