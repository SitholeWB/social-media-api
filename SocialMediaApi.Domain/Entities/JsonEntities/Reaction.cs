namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class Reaction
    {
        public int ReactionsCount { get; set; } = 0;

        // <Emoji, count>
        public IDictionary<string, int> Reactions { get; set; } = new Dictionary<string, int>();
    }
}