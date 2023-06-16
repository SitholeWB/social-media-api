namespace SocialMediaApi.Interfaces.UnitOfWork
{
    public interface IPostUnitOfWork
    {
        public IActivePostService ActivePostService { get; }

        public IPostService PostService { get; }

        public IPostCommentService PostCommentService { get; }

        public IPostReactionService PostReactionService { get; }

        public IPostCommentReactionService PostCommentReactionService { get; }

        public IEntityDetailsService EntityDetailsService { get; }
    }
}