using AutoMapper;
using HRMS.Application.DTOs;
using HRMS.Application.Contracts;
using HRMS.Application.Services;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// Document management service implementing IDocumentService
/// Handles CRUD operations for employee documents with file storage integration
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly HrmsDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        HrmsDbContext context,
        IMapper mapper,
        IFileStorage fileStorage,
        ILogger<DocumentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    /// <summary>
    /// Get all documents for the current authorized employee
    /// Employees can only see their own documents
    /// </summary>
    public async Task<IEnumerable<DocumentListResponse>> GetMyDocumentsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting documents for employee {EmployeeId}", employeeId);

        var documents = await _context.Documents
            .Where(d => d.EmployeeId == employeeId && d.IsActive)
            .Include(d => d.Employee)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<DocumentListResponse>>(documents);
    }

    /// <summary>
    /// Get all documents for a specific employee (admin/HR only)
    /// </summary>
    public async Task<IEnumerable<DocumentListResponse>> GetEmployeeDocumentsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all documents for employee {EmployeeId}", employeeId);

        var documents = await _context.Documents
            .Where(d => d.EmployeeId == employeeId)
            .Include(d => d.Employee)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<DocumentListResponse>>(documents);
    }

    /// <summary>
    /// Get all documents (admin/HR only)
    /// </summary>
    public async Task<IEnumerable<DocumentListResponse>> GetAllDocumentsAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all documents");

        var documents = await _context.Documents
            .Where(d => d.IsActive)
            .Include(d => d.Employee)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<DocumentListResponse>>(documents);
    }

    /// <summary>
    /// Search and filter documents
    /// </summary>
    public async Task<IEnumerable<DocumentListResponse>> SearchDocumentsAsync(
        Guid? employeeId,
        string? searchTerm,
        string? category,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Searching documents - EmployeeId: {EmployeeId}, SearchTerm: {SearchTerm}, Category: {Category}",
            employeeId,
            searchTerm,
            category);

        var query = _context.Documents
            .Where(d => d.IsActive)
            .Include(d => d.Employee)
            .AsQueryable();

        // Filter by employee if provided
        if (employeeId.HasValue)
        {
            query = query.Where(d => d.EmployeeId == employeeId.Value);
        }

        // Filter by search term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(d =>
                d.DocumentName.ToLower().Contains(term) ||
                d.Description!.ToLower().Contains(term) ||
                (d.Employee != null &&
                 (d.Employee.FirstName.ToLower().Contains(term) ||
                  d.Employee.LastName.ToLower().Contains(term))));
        }

        // Filter by category
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category == category);
        }

        var documents = await query
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<DocumentListResponse>>(documents);
    }

    /// <summary>
    /// Get a specific document by ID
    /// </summary>
    public async Task<DocumentDetailsResponse> GetDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting document {DocumentId}", documentId);

        var document = await _context.Documents
            .Include(d => d.Employee)
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
        {
            _logger.LogWarning("Document {DocumentId} not found", documentId);
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        return _mapper.Map<DocumentDetailsResponse>(document);
    }

    /// <summary>
    /// Upload a new document (admin/HR only)
    /// </summary>
    public async Task<DocumentDetailsResponse> UploadDocumentAsync(
        CreateDocumentRequest request,
        string currentUserEmail,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Uploading document '{DocumentName}' for employee {EmployeeId}",
            request.DocumentName,
            request.EmployeeId);

        // Verify employee exists
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Employee {EmployeeId} not found", request.EmployeeId);
            throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found");
        }

        // Upload file to storage
        using var stream = new MemoryStream(request.FileContent);
        var uploadResult = await _fileStorage.UploadAsync(
            "documents",
            $"{request.EmployeeId}/{Guid.NewGuid()}_{request.FileName}",
            stream,
            request.ContentType,
            cancellationToken);

        if (!uploadResult.Success || string.IsNullOrWhiteSpace(uploadResult.FileUrl))
        {
            _logger.LogError(
                "File upload failed for document '{DocumentName}': {ErrorMessage}",
                request.DocumentName,
                uploadResult.ErrorMessage);
            throw new InvalidOperationException(
                $"Failed to upload file: {uploadResult.ErrorMessage}");
        }

        // Create document entity
        var document = new Document
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            DocumentName = request.DocumentName,
            Category = request.Category,
            Description = request.Description,
            FileName = request.FileName,
            FileType = request.ContentType,
            FileSize = request.FileSize,
            UploadDate = DateTime.UtcNow,
            IsActive = true,
            DocumentUrl = uploadResult.FileUrl,
            UploadedByUserId = currentUserId,
            UploadedByEmail = currentUserEmail,
            CreatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document {DocumentId} uploaded successfully", document.Id);

        return _mapper.Map<DocumentDetailsResponse>(document);
    }

    /// <summary>
    /// Update document metadata (admin/HR only)
    /// </summary>
    public async Task<DocumentDetailsResponse> UpdateDocumentMetadataAsync(
        Guid documentId,
        UpdateDocumentMetadataRequest request,
        string currentUserEmail,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating metadata for document {DocumentId}", documentId);

        var document = await _context.Documents
            .Include(d => d.Employee)
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
        {
            _logger.LogWarning("Document {DocumentId} not found", documentId);
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.DocumentName))
            document.DocumentName = request.DocumentName;

        if (!string.IsNullOrWhiteSpace(request.Category))
            document.Category = request.Category;

        if (request.Description != null)
            document.Description = request.Description;

        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedByEmail = currentUserEmail;

        _context.Documents.Update(document);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document {DocumentId} metadata updated successfully", documentId);

        return _mapper.Map<DocumentDetailsResponse>(document);
    }

    /// <summary>
    /// Replace a document file (admin/HR only)
    /// </summary>
    public async Task<DocumentDetailsResponse> ReplaceDocumentFileAsync(
        Guid documentId,
        ReplaceDocumentFileRequest request,
        string currentUserEmail,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Replacing file for document {DocumentId}", documentId);

        var document = await _context.Documents
            .Include(d => d.Employee)
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
        {
            _logger.LogWarning("Document {DocumentId} not found", documentId);
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        // Delete old file if it exists
        if (!string.IsNullOrWhiteSpace(document.DocumentUrl))
        {
            // Extract container and file name from the stored path
            var pathParts = document.DocumentUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length >= 2)
            {
                await _fileStorage.DeleteAsync(
                    "documents",
                    string.Join("/", pathParts.Skip(1)),
                    cancellationToken);
            }
        }

        // Upload new file
        using var stream = new MemoryStream(request.FileContent);
        var uploadResult = await _fileStorage.UploadAsync(
            "documents",
            $"{document.EmployeeId}/{Guid.NewGuid()}_{request.FileName}",
            stream,
            request.ContentType,
            cancellationToken);

        if (!uploadResult.Success || string.IsNullOrWhiteSpace(uploadResult.FileUrl))
        {
            _logger.LogError(
                "File replacement failed for document {DocumentId}: {ErrorMessage}",
                documentId,
                uploadResult.ErrorMessage);
            throw new InvalidOperationException(
                $"Failed to upload replacement file: {uploadResult.ErrorMessage}");
        }

        // Update document
        document.FileName = request.FileName;
        document.FileType = request.ContentType;
        document.FileSize = request.FileSize;
        document.UploadDate = DateTime.UtcNow;
        document.DocumentUrl = uploadResult.FileUrl;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedByEmail = currentUserEmail;

        _context.Documents.Update(document);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document {DocumentId} file replaced successfully", documentId);

        return _mapper.Map<DocumentDetailsResponse>(document);
    }

    /// <summary>
    /// Delete a document (admin/HR only)
    /// </summary>
    public async Task DeleteDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting document {DocumentId}", documentId);

        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
        {
            _logger.LogWarning("Document {DocumentId} not found", documentId);
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        // Delete file from storage
        if (!string.IsNullOrWhiteSpace(document.DocumentUrl))
        {
            // Extract container and file name from the stored path
            var pathParts = document.DocumentUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length >= 2)
            {
                await _fileStorage.DeleteAsync(
                    "documents",
                    string.Join("/", pathParts.Skip(1)),
                    cancellationToken);
            }
        }

        // Delete from database
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document {DocumentId} deleted successfully", documentId);
    }

    /// <summary>
    /// Download a document file
    /// </summary>
    public async Task<(byte[] FileContent, string FileName, string ContentType)> DownloadDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading document {DocumentId}", documentId);

        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
        {
            _logger.LogWarning("Document {DocumentId} not found", documentId);
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        // Download from file storage
        var fileStream = await _fileStorage.DownloadAsync(
            "documents",
            $"{document.EmployeeId}/{Path.GetFileName(document.DocumentUrl)}",
            cancellationToken);

        if (fileStream == null)
        {
            throw new InvalidOperationException($"Failed to download document {documentId}");
        }

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        await fileStream.DisposeAsync();

        return (memoryStream.ToArray(), document.FileName, document.FileType);
    }

    /// <summary>
    /// Get a preview URL for a document
    /// </summary>
    public async Task<string> GetPreviewUrlAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting preview URL for document {DocumentId}", documentId);

        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
        {
            _logger.LogWarning("Document {DocumentId} not found", documentId);
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        // Return preview URL or generate signed URL if using cloud storage
        return document.DocumentUrl ?? throw new InvalidOperationException(
            $"Document {documentId} does not have a valid URL");
    }
}
