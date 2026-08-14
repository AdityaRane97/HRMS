using HRMS.Application.Contracts;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory implementation of file storage for Phase 1 development.
/// Useful for local testing without Azure/AWS/GCP dependencies.
/// Will be replaced with cloud provider implementations for production.
/// </summary>
public class InMemoryFileStorage : IFileStorage
{
    private readonly Dictionary<string, Dictionary<string, StoredFile>> _containers = new();
    private readonly object _lockObject = new();

    private class StoredFile
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string? ContentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
    }

    public Task<FileStorageResult> UploadAsync(
        string containerName,
        string fileName,
        Stream fileContent,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (!_containers.ContainsKey(containerName))
                _containers[containerName] = new Dictionary<string, StoredFile>();

            using var memoryStream = new MemoryStream();
            fileContent.CopyTo(memoryStream);
            var content = memoryStream.ToArray();

            _containers[containerName][fileName] = new StoredFile
            {
                Content = content,
                ContentType = contentType,
                CreatedAt = DateTime.UtcNow
            };

            return Task.FromResult(new FileStorageResult
            {
                Success = true,
                FileId = Guid.NewGuid().ToString(),
                FileUrl = $"memory://{containerName}/{fileName}",
                FileSize = content.Length,
                ContentType = contentType
            });
        }
    }

    public Task<Stream?> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_containers.TryGetValue(containerName, out var container) &&
                container.TryGetValue(fileName, out var file))
            {
                var stream = new MemoryStream(file.Content);
                return Task.FromResult<Stream?>(stream);
            }
        }

        return Task.FromResult<Stream?>(null);
    }

    public Task<bool> DeleteAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_containers.TryGetValue(containerName, out var container))
            {
                return Task.FromResult(container.Remove(fileName));
            }
        }

        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_containers.TryGetValue(containerName, out var container))
            {
                return Task.FromResult(container.ContainsKey(fileName));
            }
        }

        return Task.FromResult(false);
    }

    public Task<string?> GenerateSecureUrlAsync(
        string containerName,
        string fileName,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_containers.TryGetValue(containerName, out var container) &&
                container.ContainsKey(fileName))
            {
                // For Phase 1, generate a simple token-like URL
                var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{containerName}:{fileName}:{DateTime.UtcNow.Ticks}"));
                return Task.FromResult<string?>($"memory-secure://download/{token}");
            }
        }

        return Task.FromResult<string?>(null);
    }

    public Task<FileMetadata?> GetFileMetadataAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_containers.TryGetValue(containerName, out var container) &&
                container.TryGetValue(fileName, out var file))
            {
                return Task.FromResult<FileMetadata?>(new FileMetadata
                {
                    FileName = fileName,
                    SizeInBytes = file.Content.Length,
                    CreatedAt = file.CreatedAt,
                    ModifiedAt = file.ModifiedAt,
                    ContentType = file.ContentType
                });
            }
        }

        return Task.FromResult<FileMetadata?>(null);
    }
}
