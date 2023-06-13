﻿using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.Entities
{
    public class GroupPost : BasePost
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; } = default!;
        public virtual ICollection<GroupPostComment> GroupPostComments { get; set; } = new List<GroupPostComment>();
    }
}