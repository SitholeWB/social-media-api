namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class Reaction
    {
        public int ReactionsCount { get; set; } = 0;

        public IList<Emoji> Emojis { get; set; } = new List<Emoji>();
    }
}