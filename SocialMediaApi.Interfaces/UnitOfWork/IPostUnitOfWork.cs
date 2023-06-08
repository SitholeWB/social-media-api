namespace SocialMediaApi.Interfaces.UnitOfWork
{
    public interface IPostUnitOfWork
    {
        public IActiveGroupPostService ActiveGroupPostService { get; }

        public INewGroupPostService NewGroupPostService { get; }

        public IGroupPostService GroupPostService { get; }
    }
}