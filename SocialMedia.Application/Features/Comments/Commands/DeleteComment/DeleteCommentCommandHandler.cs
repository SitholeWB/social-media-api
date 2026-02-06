namespace SocialMedia.Application;

public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, bool>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IDispatcher _dispatcher;

    public DeleteCommentCommandHandler(ICommentRepository commentRepository, IDispatcher dispatcher)
    {
        _commentRepository = commentRepository;
        _dispatcher = dispatcher;
    }

    public async Task<bool> HandleAsync(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (comment == null || command.UserId != comment.AuthorId)
        {
            return false;
        }
        comment.IsDeleted = true;
        await _commentRepository.UpdateAsync(comment, cancellationToken);
        await _dispatcher.PublishAsync(new CommentDeletedEvent(comment), cancellationToken);
        return true;
    }
}