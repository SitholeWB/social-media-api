using NSubstitute;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Logic.EventHandlers;
using SocialMediaApi.Domain.Logic.Services;
using SocialMediaApi.Domain.Models.Groups;
using SocialMediaApi.Domain.Models.Security;

namespace SocialMediaApi.Domain.Logic.Tests
{
	public class Tests
	{
		private SocialMediaApiDbContext _context;
		private IGroupService _groupService;
		private AuthUser _authUser;

		[SetUp]
		public async Task Setup()
		{
			_authUser = new AuthUser
			{
				AuthorizedUser = new BaseUser { },
				AuthenticatedUser = new BaseUser { }
			};
			_context = DbContextHelper.GetDatabaseContext();
			await _context.BasicSetup();
			var _authService = Substitute.For<IAuthService>();
			var _serviceProvider = Substitute.For<IServiceProvider>();
			var _publisher = new EventHandlerContainer(_serviceProvider);

			//_authService.GetAuthorizedUser().Returns(x => DbContextHelper._creator);

			_groupService = new GroupService(_context, _publisher);
		}

		[TearDown]
		public async Task Teardown()
		{
			await _context.DisposeAsync();
		}

		[Test]
		public async Task AddGroupAsync_GivenValidInput_ShouldAddGroup()
		{
			var addedGroup = await _groupService.AddGroupAsync(_authUser, new AddGroupModel
			{
				Description = "This is test group",
				Name = "Test Group 1"
			});
			Assert.Multiple(() =>
			{
				Assert.That(addedGroup.Description, Is.EqualTo("This is test group"));
				Assert.That(addedGroup.Name, Is.EqualTo("Test Group 1"));
			});
		}

		[Test]
		public async Task AddGroupAsync_GivenValidEmptyDescription_ShouldAddGroup()
		{
			var addedGroup = await _groupService.AddGroupAsync(_authUser, new AddGroupModel
			{
				Description = string.Empty,
				Name = "Test Group 1"
			});
			Assert.Multiple(() =>
			{
				Assert.That(addedGroup.Description, Is.EqualTo(string.Empty));
				Assert.That(addedGroup.Name, Is.EqualTo("Test Group 1"));
			});
		}

		[Test]
		public async Task AddGroupAsync_GivenValidNullDescription_ShouldAddGroup()
		{
			var addedGroup = await _groupService.AddGroupAsync(_authUser, new AddGroupModel
			{
				Description = null,
				Name = "Test Group 1"
			});
			Assert.Multiple(() =>
			{
				Assert.That(addedGroup.Description, Is.EqualTo(string.Empty));
				Assert.That(addedGroup.Name, Is.EqualTo("Test Group 1"));
			});
		}

		[Test]
		public void AddGroupAsync_GivenValidEmptyName_ShouldThrowException()
		{
			Assert.ThrowsAsync<SocialMediaException>(async () => await _groupService.AddGroupAsync(_authUser, new AddGroupModel
			{
				Description = "This is test group",
				Name = string.Empty
			}));
		}

		[Test]
		public void AddGroupAsync_GivenValidNullName_ShouldThrowException()
		{
			Assert.ThrowsAsync<SocialMediaException>(async () => await _groupService.AddGroupAsync(_authUser, new AddGroupModel
			{
				Description = "This is test group",
				Name = null
			}));
		}
	}
}