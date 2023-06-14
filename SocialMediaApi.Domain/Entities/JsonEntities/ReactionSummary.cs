namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class ReactionSummary
    {
        public int ReactionsCount { get; set; } = 0;

        public List<Emoji> Emojis { get; set; } = new List<Emoji>();
    }
}