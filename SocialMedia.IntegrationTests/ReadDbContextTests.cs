using Microsoft.EntityFrameworkCore;
using SocialMedia.Infrastructure;
using Xunit;

namespace SocialMedia.IntegrationTests;

public class ReadDbContextTests
{
    [Fact]
    public void Can_Create_ReadDbContext_And_Build_Model()
    {
        var options = new DbContextOptionsBuilder<SocialMediaReadDbContext>()
            .UseInMemoryDatabase(databaseName: "ReadDbTest")
            .Options;

        using var context = new SocialMediaReadDbContext(options);

        // Force model creation
        var model = context.Model;

        Assert.NotNull(model);

        var postEntity = model.FindEntityType("SocialMedia.Domain.PostReadModel");
        Assert.NotNull(postEntity);

        // Verify JSON ownerships
        var statsNav = postEntity.FindNavigation("Stats");
        Assert.NotNull(statsNav);
        Assert.True(statsNav.TargetEntityType.IsOwned());

        var reactionsNav = postEntity.FindNavigation("Reactions");
        Assert.NotNull(reactionsNav);
        Assert.True(reactionsNav.TargetEntityType.IsOwned());

        var commentEntity = model.FindEntityType("SocialMedia.Domain.CommentReadModel");
        Assert.NotNull(commentEntity);

        var commentStatsNav = commentEntity.FindNavigation("Stats");
        Assert.NotNull(commentStatsNav);
        Assert.True(commentStatsNav.TargetEntityType.IsOwned());
    }
}
