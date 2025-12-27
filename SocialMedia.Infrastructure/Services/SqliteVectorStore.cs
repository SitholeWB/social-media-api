using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SocialMedia.Infrastructure;

public class SqliteVectorStore
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteVectorStore> _logger;

    public SqliteVectorStore(IConfiguration configuration, ILogger<SqliteVectorStore> logger)
    {
        _connectionString = configuration.GetConnectionString("VectorDb") ?? "Data Source=vectors.db";
        _logger = logger;
        InitializeAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var createTableSql = @"
            CREATE TABLE IF NOT EXISTS PostVectors (
                PostId TEXT PRIMARY KEY,
                Content TEXT,
                Embedding BLOB,
                CreatedAt TEXT
            )";

        using var command = new SqliteCommand(createTableSql, connection);
        await command.ExecuteNonQueryAsync();

        var createInteractionsTableSql = @"
            CREATE TABLE IF NOT EXISTS UserInteractions (
                UserId TEXT,
                PostId TEXT,
                CreatedAt TEXT,
                PRIMARY KEY (UserId, PostId)
            )";
        using var command2 = new SqliteCommand(createInteractionsTableSql, connection);
        await command2.ExecuteNonQueryAsync();
    }

    public async Task UpsertAsync(PostVectorRecord record)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var upsertSql = @"
            INSERT INTO PostVectors (PostId, Content, Embedding, CreatedAt)
            VALUES ($postId, $content, $embedding, $createdAt)
            ON CONFLICT(PostId) DO UPDATE SET
                Content = excluded.Content,
                Embedding = excluded.Embedding,
                CreatedAt = excluded.CreatedAt";

        using var command = new SqliteCommand(upsertSql, connection);
        command.Parameters.AddWithValue("$postId", record.PostId.ToString());
        command.Parameters.AddWithValue("$content", record.Content);

        // Convert ReadOnlyMemory<float> to byte array
        var embeddingBytes = new byte[record.Embedding.Length * sizeof(float)];
        Buffer.BlockCopy(record.Embedding.ToArray(), 0, embeddingBytes, 0, embeddingBytes.Length);
        command.Parameters.AddWithValue("$embedding", embeddingBytes);

        command.Parameters.AddWithValue("$createdAt", record.CreatedAt.ToString("O"));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var deleteSql = "DELETE FROM PostVectors WHERE PostId = $postId";
        using var command = new SqliteCommand(deleteSql, connection);
        command.Parameters.AddWithValue("$postId", postId.ToString());

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Guid>> SearchAsync(ReadOnlyMemory<float> queryVector, int topK)
    {
        var allRecords = new List<(Guid PostId, float[] Embedding)>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var selectSql = "SELECT PostId, Embedding FROM PostVectors";
        using var command = new SqliteCommand(selectSql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var postId = Guid.Parse(reader.GetString(0));
            var embeddingBytes = (byte[])reader.GetValue(1);
            var embedding = new float[embeddingBytes.Length / sizeof(float)];
            Buffer.BlockCopy(embeddingBytes, 0, embedding, 0, embeddingBytes.Length);

            allRecords.Add((postId, embedding));
        }

        if (allRecords.Count == 0) return new List<Guid>();

        // Manual cosine similarity since SQLite doesn't have vector support out of the box without
        // specialized extensions (like sqlite-vss) which might be harder to set up here.
        var results = allRecords
            .Select(record => new
            {
                record.PostId,
                Similarity = CosineSimilarity(queryVector.Span, record.Embedding)
            })
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.PostId)
            .ToList();

        return results;
    }

    private static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        if (a.Length != b.Length) return 0f;

        float dotProduct = 0f;
        float magnitudeA = 0f;
        float magnitudeB = 0f;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0) return 0f;

        return dotProduct / (MathF.Sqrt(magnitudeA) * MathF.Sqrt(magnitudeB));
    }

    public async Task RecordUserInteractionAsync(Guid userId, Guid postId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO UserInteractions (UserId, PostId, CreatedAt)
            VALUES ($userId, $postId, $createdAt)
            ON CONFLICT(UserId, PostId) DO UPDATE SET
                CreatedAt = excluded.CreatedAt";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("$userId", userId.ToString());
        command.Parameters.AddWithValue("$postId", postId.ToString());
        command.Parameters.AddWithValue("$createdAt", DateTimeOffset.UtcNow.ToString("O"));

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<float[]>> GetUserInteractionEmbeddingsAsync(Guid userId, int limit)
    {
        var embeddings = new List<float[]>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // Join Interactions with PostVectors to get embeddings of perceived 'good' posts
        // Order by most recent interaction
        var sql = @"
            SELECT pv.Embedding 
            FROM UserInteractions ui
            JOIN PostVectors pv ON ui.PostId = pv.PostId
            WHERE ui.UserId = $userId
            ORDER BY ui.CreatedAt DESC
            LIMIT $limit";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("$userId", userId.ToString());
        command.Parameters.AddWithValue("$limit", limit);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var embeddingBytes = (byte[])reader.GetValue(0);
            var embedding = new float[embeddingBytes.Length / sizeof(float)];
            Buffer.BlockCopy(embeddingBytes, 0, embedding, 0, embeddingBytes.Length);
            embeddings.Add(embedding);
        }

        return embeddings;
    }
}

// VectorData property mapping for future extensibility as requested
public class PostVectorData
{
    public Guid PostId { get; set; }

    public string Content { get; set; } = string.Empty;

    public ReadOnlyMemory<float> Embedding { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}