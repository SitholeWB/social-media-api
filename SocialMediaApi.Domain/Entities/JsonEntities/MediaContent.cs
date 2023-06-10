﻿using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class MediaContent
    {
        [Required]
        public string Url { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;
    }
}