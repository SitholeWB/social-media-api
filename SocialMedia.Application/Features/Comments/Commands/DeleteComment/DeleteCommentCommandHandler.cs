namespace SocialMedia.Application;

public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, bool>
{
    private readonly ICommentRepository _commentRepository;

    public DeleteCommentCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<bool> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (comment == null)
        {
            return false;
        }

        await _commentRepository.DeleteAsync(comment, cancellationToken);
        return true;
    }
}
