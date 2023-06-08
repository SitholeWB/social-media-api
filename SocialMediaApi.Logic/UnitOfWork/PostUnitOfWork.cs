using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActiveGroupPostService _activeGroupPostService;
        private readonly INewGroupPostService _newGroupPostService;
        private readonly IGroupPostService _groupPostService;

        public PostUnitOfWork(IActiveGroupPostService activeGroupPostService, INewGroupPostService newGroupPostService, IGroupPostService groupPostService)
        {
            _activeGroupPostService = activeGroupPostService;
            _newGroupPostService = newGroupPostService;
            _groupPostService = groupPostService;
        }

        public IActiveGroupPostService ActiveGroupPostService => _activeGroupPostService;

        public INewGroupPostService NewGroupPostService => _newGroupPostService;

        public IGroupPostService GroupPostService => _groupPostService;
    }
}