namespace SocialMediaApi.Interfaces.UnitOfWork
{
    public interface IPostUnitOfWork
    {
        public IActivePostService ActivePostService { get; }

        public IPostService PostService { get; }

        public ICommentService CommentService { get; }

        public IPostReactionService PostReactionService { get; }

        public ICommentReactionService CommentReactionService { get; }

        public IEntityDetailsService EntityDetailsService { get; }

        public IUserDetailsService UserDetailsService { get; }

        public IUserPostService UserPostService { get; }

        public IUserGroupService UserGroupService { get; }
    }
}