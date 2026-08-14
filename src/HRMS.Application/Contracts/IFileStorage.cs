namespace HRMS.Application.Contracts;

/// <summary>
/// Cloud-neutral file storage abstraction.
/// Supports Azure Blob Storage, AWS S3, Google Cloud Storage, and local file system.
/// Implementation is selected based on configuration and deployment target.
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Upload a file to storage.
    /// </summary>
    Task<FileStorageResult> UploadAsync(
        string containerName,
        string fileName,
        Stream fileContent,
        string? contentType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file from storage.
    /// </summary>
    Task<Stream?> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from storage.
    /// </summary>
    Task<bool> DeleteAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a file exists.
    /// </summary>
    Task<bool> ExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a secure URL for file download.
    /// URL should be time-limited and require authorization.
    /// Do NOT return permanent public URLs for sensitive documents.
    /// </summary>
    Task<string?> GenerateSecureUrlAsync(
        string containerName,
        string fileName,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get file metadata (size, modified date, etc).
    /// </summary>
    Task<FileMetadata?> GetFileMetadataAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of file upload operation.
/// </summary>
public class FileStorageResult
{
    public bool Success { get; set; }
    public string? FileId { get; set; }
    public string? FileUrl { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// File metadata.
/// </summary>
public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ContentType { get; set; }
}
