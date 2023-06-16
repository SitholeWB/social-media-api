namespace SocialMediaApi.Interfaces.UnitOfWork
{
    public interface IPostUnitOfWork
    {
        public IActiveGroupPostService ActiveGroupPostService { get; }

        public IGroupPostService GroupPostService { get; }

        public IGroupPostCommentService GroupPostCommentService { get; }
        public IPostReactionService IPostReactionService { get; }
        public IEntityDetailsService IEntityDetailsService { get; }
    }
}