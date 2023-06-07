namespace SocialMediaApi.Domain.Entities
{
    public class ActiveGroupPost : GroupPost
    {
        public int Rank { get; set; }
        //public List<MiniComment> Comments { get; set; } = new List<MiniComment>();
    }
}