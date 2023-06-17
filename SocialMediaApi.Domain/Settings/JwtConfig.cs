﻿namespace SocialMediaApi.Domain.Settings
{
    public class JwtConfig
    {
        public bool ValidateIssuerSigningKey { get; set; }

        public string IssuerSigningKey { get; set; } = "issue--signing--key----here----";

        public bool ValidateIssuer { get; set; } = true;

        public string ValidIssuer { get; set; }

        public bool ValidateAudience { get; set; } = true;

        public string ValidAudience { get; set; }

        public bool RequireExpirationTime { get; set; }

        public bool ValidateLifetime { get; set; } = true;
    }
}