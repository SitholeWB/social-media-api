
namespace SocialMedia.Infrastructure;

public class BlockchainService : IBlockchainService
{
    private readonly SocialMediaDbContext _dbContext;

    public BlockchainService(SocialMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddVoteAsync(Guid voteId, Guid userId, Guid pollOptionId, CancellationToken cancellationToken = default)
    {
        var lastBlock = await _dbContext.Set<Block>()
            .OrderByDescending(b => b.Index)
            .FirstOrDefaultAsync(cancellationToken);

        var newBlock = new Block
        {
            Id = Guid.NewGuid(),
            Index = lastBlock != null ? lastBlock.Index + 1 : 0,
            Timestamp = DateTime.UtcNow,
            VoteId = voteId,
            PreviousHash = lastBlock != null ? lastBlock.Hash : "0",
            Nonce = 0
        };

        // Simple "mining" or just hashing
        newBlock.Hash = CalculateHash(newBlock);

        await _dbContext.Set<Block>().AddAsync(newBlock, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> VerifyChainAsync(CancellationToken cancellationToken = default)
    {
        var blocks = await _dbContext.Set<Block>()
            .OrderBy(b => b.Index)
            .ToListAsync(cancellationToken);

        for (int i = 0; i < blocks.Count; i++)
        {
            var currentBlock = blocks[i];
            var previousBlock = i > 0 ? blocks[i - 1] : null;

            // 1. Recalculate hash and compare
            if (currentBlock.Hash != CalculateHash(currentBlock))
            {
                return false;
            }

            // 2. Check previous hash link
            if (previousBlock != null && currentBlock.PreviousHash != previousBlock.Hash)
            {
                return false;
            }

            // 3. Check genesis block
            if (previousBlock == null && currentBlock.PreviousHash != "0")
            {
                return false;
            }
        }

        return true;
    }

    private string CalculateHash(Block block)
    {
        using (var sha256 = SHA256.Create())
        {
            var rawData = $"{block.Index}{block.Timestamp:O}{block.VoteId}{block.PreviousHash}{block.Nonce}";
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
