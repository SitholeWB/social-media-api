namespace SocialMediaApi.Interfaces.UnitOfWork
{
    public interface IPostUnitOfWork
    {
        public IActiveGroupPostService ActiveGroupPostService { get; }

        public IGroupPostService GroupPostService { get; }
    }
}