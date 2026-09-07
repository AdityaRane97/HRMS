using HRMS.Application.DTOs;

namespace HRMS.Application.Services;

/// <summary>
/// Service interface for document management operations
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// Get all documents for the current authorized employee (employees can only see their own documents)
    /// </summary>
    Task<IEnumerable<DocumentListResponse>> GetMyDocumentsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all documents for a specific employee (admin/HR only)
    /// </summary>
    Task<IEnumerable<DocumentListResponse>> GetEmployeeDocumentsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all documents (admin/HR only)
    /// </summary>
    Task<IEnumerable<DocumentListResponse>> GetAllDocumentsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search and filter documents
    /// For employees: only their documents
    /// For admin/HR: all documents
    /// </summary>
    Task<IEnumerable<DocumentListResponse>> SearchDocumentsAsync(
        Guid? employeeId,
        string? searchTerm,
        string? category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific document by ID
    /// </summary>
    Task<DocumentDetailsResponse> GetDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a new document (admin/HR only)
    /// </summary>
    Task<DocumentDetailsResponse> UploadDocumentAsync(
        CreateDocumentRequest request,
        string currentUserEmail,
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update document metadata (admin/HR only)
    /// </summary>
    Task<DocumentDetailsResponse> UpdateDocumentMetadataAsync(
        Guid documentId,
        UpdateDocumentMetadataRequest request,
        string currentUserEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Replace a document file (admin/HR only)
    /// </summary>
    Task<DocumentDetailsResponse> ReplaceDocumentFileAsync(
        Guid documentId,
        ReplaceDocumentFileRequest request,
        string currentUserEmail,
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a document (admin/HR only)
    /// </summary>
    Task DeleteDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a document file
    /// </summary>
    Task<(byte[] FileContent, string FileName, string ContentType)> DownloadDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a preview URL for a document
    /// </summary>
    Task<string> GetPreviewUrlAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}
