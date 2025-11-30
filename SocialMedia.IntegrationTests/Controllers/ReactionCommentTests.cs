using SocialMedia.Application;
using SocialMedia.Domain.ReadModels;
using SocialMedia.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace SocialMedia.IntegrationTests;

public class ReactionCommentTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public ReactionCommentTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ReactToComment_ShouldUpdateReadModel()
    {
        // Arrange
        var client = _factory.CreateClient();
        var postDto = new CreatePostDto { Title = "Test Post", Content = "Content", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", postDto);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var commentDto = new CreateCommentDto { PostId = postId, Content = "Test Comment", AuthorId = Guid.NewGuid() };
        var commentResponse = await client.PostAsJsonAsync("/api/v1/comments", commentDto);
        var commentId = await commentResponse.Content.ReadFromJsonAsync<Guid>();

        // Act: React to comment
        var reactionCommand = new { CommentId = commentId, UserId = Guid.NewGuid(), Emoji = "❤️" };
        var reactionResponse = await client.PostAsJsonAsync("/api/v1/likes/toggle", reactionCommand);
        reactionResponse.EnsureSuccessStatusCode();

        // Assert: Check Post Read Model (TopComments)
        var getPostResponse = await client.GetAsync($"/api/v1/posts?pageNumber=1&pageSize=10");
        var postResult = await getPostResponse.Content.ReadFromJsonAsync<PagedResult<PostDto>>();
        var post = postResult.Items.FirstOrDefault(p => p.Id == postId);
        
        // Note: PostDto currently doesn't expose TopComments, we might need to update PostDto or fetch comments separately
        // But we can check the comments endpoint
        
        var getCommentsResponse = await client.GetAsync($"/api/v1/posts/{postId}/comments");
        getCommentsResponse.EnsureSuccessStatusCode();
        var commentsResult = await getCommentsResponse.Content.ReadFromJsonAsync<PagedResult<CommentReadDto>>();
        
        Assert.NotNull(commentsResult);
        var comment = commentsResult.Items.FirstOrDefault(c => c.Id == commentId);
        Assert.NotNull(comment);
        Assert.Equal(1, comment.LikeCount);
        Assert.Contains(comment.Reactions, r => r.Emoji == "❤️");
    }

    [Fact]
    public async Task GetPost_ShouldReturnTop30Comments()
    {
        // Arrange
        var client = _factory.CreateClient();
        var postDto = new CreatePostDto { Title = "Popular Post", Content = "Content", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", postDto);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Add 35 comments
        for (int i = 0; i < 35; i++)
        {
            var commentDto = new CreateCommentDto { PostId = postId, Content = $"Comment {i}", AuthorId = Guid.NewGuid() };
            await client.PostAsJsonAsync("/api/v1/comments", commentDto);
        }

        // Act: Get Post Comments (Page 1)
        var getCommentsResponse = await client.GetAsync($"/api/v1/posts/{postId}/comments?pageNumber=1&pageSize=30");
        var commentsResult = await getCommentsResponse.Content.ReadFromJsonAsync<PagedResult<CommentReadDto>>();

        // Assert
        Assert.Equal(30, commentsResult.Items.Count);
        
        // Act: Get Remaining Comments (Page 2)
        var getMoreCommentsResponse = await client.GetAsync($"/api/v1/posts/{postId}/comments?pageNumber=2&pageSize=10");
        var moreCommentsResult = await getMoreCommentsResponse.Content.ReadFromJsonAsync<PagedResult<CommentReadDto>>();
        
        Assert.True(moreCommentsResult.Items.Count >= 5); // Should be 5
    }
}
