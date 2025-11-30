namespace SocialMedia.Application;

public class UpdateCommentCommandHandler : ICommandHandler<UpdateCommentCommand, bool>
{
    private readonly ICommentRepository _commentRepository;

    public UpdateCommentCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<bool> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (comment == null)
        {
            return false;
        }

        comment.Content = command.Content;
        await _commentRepository.UpdateAsync(comment, cancellationToken);
        return true;
    }
}
