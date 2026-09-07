namespace HRMS.Application.DTOs;

/// <summary>
/// Data Transfer Object for Document entity
/// </summary>
public class DocumentDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DocumentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }

    public bool IsActive { get; set; }

    public string? DocumentUrl { get; set; }

    public string? UploadedByEmail { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedByEmail { get; set; }
}

/// <summary>
/// Request DTO for creating/uploading a document
/// </summary>
public class CreateDocumentRequest
{
    public Guid EmployeeId { get; set; }

    public string DocumentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// File to upload (set by controller from IFormFile)
    /// </summary>
    public byte[] FileContent { get; set; } = [];

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }
}

/// <summary>
/// Request DTO for updating document metadata
/// </summary>
public class UpdateDocumentMetadataRequest
{
    public string? DocumentName { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }
}

/// <summary>
/// Request DTO for replacing a document file
/// </summary>
public class ReplaceDocumentFileRequest
{
    public byte[] FileContent { get; set; } = [];

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }
}

/// <summary>
/// Response DTO for document listing
/// </summary>
public class DocumentListResponse
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DocumentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }

    public string? UploadedByEmail { get; set; }
}

/// <summary>
/// Response DTO for document details
/// </summary>
public class DocumentDetailsResponse
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DocumentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }

    public bool IsActive { get; set; }

    public string? UploadedByEmail { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedByEmail { get; set; }
}
