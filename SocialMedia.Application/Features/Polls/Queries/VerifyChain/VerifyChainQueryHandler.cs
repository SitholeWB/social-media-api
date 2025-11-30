namespace SocialMedia.Application;

public class VerifyChainQueryHandler : IQueryHandler<VerifyChainQuery, bool>
{
    private readonly IBlockchainService _blockchainService;

    public VerifyChainQueryHandler(IBlockchainService blockchainService)
    {
        _blockchainService = blockchainService;
    }

    public async Task<bool> Handle(VerifyChainQuery query, CancellationToken cancellationToken)
    {
        return await _blockchainService.VerifyChainAsync(cancellationToken);
    }
}
