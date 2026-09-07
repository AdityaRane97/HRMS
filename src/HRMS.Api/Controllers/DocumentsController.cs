using HRMS.Application.Constants;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using HRMS.Api.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers;

/// <summary>
/// Document management API endpoints
/// Supports viewing, uploading, updating, and deleting employee documents
/// Role-based authorization: Employees see only their documents, HR/Admin manage all documents
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly CurrentUser _currentUser;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentService documentService,
        CurrentUser currentUser,
        ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Get documents for the current employee (my documents)
    /// Employees can only access their own documents
    /// </summary>
    [HttpGet("my-documents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DocumentListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DocumentListResponse>>>> GetMyDocuments(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting documents for current employee {EmployeeId}",
            _currentUser.EmployeeId);

        try
        {
            var documents = await _documentService.GetMyDocumentsAsync(
                _currentUser.EmployeeId,
                cancellationToken);

            return Ok(new ApiResponse<IEnumerable<DocumentListResponse>>
            {
                Success = true,
                Message = "Documents retrieved successfully",
                Data = documents
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents for employee {EmployeeId}", 
                _currentUser.EmployeeId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<IEnumerable<DocumentListResponse>>
                {
                    Success = false,
                    Message = "Failed to retrieve documents"
                });
        }
    }

    /// <summary>
    /// Get all documents (admin/HR only)
    /// Returns paginated list of all employee documents
    /// </summary>
    [HttpGet]
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DocumentListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DocumentListResponse>>>> GetAllDocuments(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all documents");

        try
        {
            var documents = await _documentService.GetAllDocumentsAsync(cancellationToken);

            return Ok(new ApiResponse<IEnumerable<DocumentListResponse>>
            {
                Success = true,
                Message = "Documents retrieved successfully",
                Data = documents
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all documents");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<IEnumerable<DocumentListResponse>>
                {
                    Success = false,
                    Message = "Failed to retrieve documents"
                });
        }
    }

    /// <summary>
    /// Get documents for a specific employee (admin/HR only)
    /// Always uses the JWT authenticated user ID for authorization, not the employee ID from request
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DocumentListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DocumentListResponse>>>> GetEmployeeDocuments(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting documents for employee {EmployeeId} by {CurrentUserId}",
            employeeId,
            _currentUser.EmployeeId);

        try
        {
            var documents = await _documentService.GetEmployeeDocumentsAsync(
                employeeId,
                cancellationToken);

            return Ok(new ApiResponse<IEnumerable<DocumentListResponse>>
            {
                Success = true,
                Message = "Documents retrieved successfully",
                Data = documents
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<IEnumerable<DocumentListResponse>>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents for employee {EmployeeId}", employeeId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<IEnumerable<DocumentListResponse>>
                {
                    Success = false,
                    Message = "Failed to retrieve documents"
                });
        }
    }

    /// <summary>
    /// Search and filter documents
    /// Employees see their own documents; Admin/HR see all documents
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DocumentListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DocumentListResponse>>>> SearchDocuments(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] Guid? employeeId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Searching documents - SearchTerm: {SearchTerm}, Category: {Category}, EmployeeId: {EmployeeId}",
            searchTerm,
            category,
            employeeId);

        try
        {
            // For employees, always search only their documents
            var searchEmployeeId = User.IsInRole(RoleConstants.HROrAdmin)
                ? employeeId
                : _currentUser.EmployeeId;

            var documents = await _documentService.SearchDocumentsAsync(
                searchEmployeeId,
                searchTerm,
                category,
                cancellationToken);

            return Ok(new ApiResponse<IEnumerable<DocumentListResponse>>
            {
                Success = true,
                Message = "Search completed successfully",
                Data = documents
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching documents");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<IEnumerable<DocumentListResponse>>
                {
                    Success = false,
                    Message = "Failed to search documents"
                });
        }
    }

    /// <summary>
    /// Get a specific document by ID
    /// Employees can only access their own documents; Admin/HR can access any document
    /// </summary>
    [HttpGet("{documentId}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DocumentDetailsResponse>>> GetDocument(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting document {DocumentId}", documentId);

        try
        {
            var document = await _documentService.GetDocumentAsync(
                documentId,
                cancellationToken);

            // Authorization check: Employees can only access their own documents
            if (!User.IsInRole(RoleConstants.HROrAdmin) && 
                document.EmployeeId != _currentUser.EmployeeId)
            {
                _logger.LogWarning(
                    "Unauthorized access attempt to document {DocumentId} by employee {EmployeeId}",
                    documentId,
                    _currentUser.EmployeeId);

                return Forbid();
            }

            return Ok(new ApiResponse<DocumentDetailsResponse>
            {
                Success = true,
                Message = "Document retrieved successfully",
                Data = document
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<DocumentDetailsResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {DocumentId}", documentId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "Failed to retrieve document"
                });
        }
    }

    /// <summary>
    /// Upload a new document (admin/HR only)
    /// Backend ALWAYS uses the JWT authenticated user ID for authorization, not the request body
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DocumentDetailsResponse>>> UploadDocument(
        [FromForm] UploadDocumentForm request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Uploading document for employee {EmployeeId} by authenticated user {CurrentUserId}",
            request.EmployeeId,
            _currentUser.EmployeeId);

        try
        {
            // Validate file
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "No file provided"
                });
            }

            if (!IsValidFileFormat(request.File.ContentType))
            {
                return BadRequest(new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "Invalid file format. Allowed formats: PDF, Word, Excel, Images"
                });
            }

            // Read file content
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream, cancellationToken);

            var createRequest = new CreateDocumentRequest
            {
                EmployeeId = request.EmployeeId,
                DocumentName = request.DocumentName,
                Category = request.Category,
                Description = request.Description,
                FileContent = memoryStream.ToArray(),
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length
            };

            // Upload document using JWT authenticated user info
            var document = await _documentService.UploadDocumentAsync(
                createRequest,
                _currentUser.Email ?? "unknown@example.com",
                _currentUser.EmployeeId,
                cancellationToken);

            return CreatedAtAction(nameof(GetDocument), 
                new { documentId = document.Id },
                new ApiResponse<DocumentDetailsResponse>
                {
                    Success = true,
                    Message = "Document uploaded successfully",
                    Data = document
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<DocumentDetailsResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "Failed to upload document"
                });
        }
    }

    /// <summary>
    /// Update document metadata (admin/HR only)
    /// </summary>
    [HttpPut("{documentId}/metadata")]
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [ProducesResponseType(typeof(ApiResponse<DocumentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DocumentDetailsResponse>>> UpdateDocumentMetadata(
        Guid documentId,
        [FromBody] UpdateDocumentMetadataRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating metadata for document {DocumentId} by user {CurrentUserId}",
            documentId,
            _currentUser.EmployeeId);

        try
        {
            var document = await _documentService.UpdateDocumentMetadataAsync(
                documentId,
                request,
                _currentUser.Email ?? "unknown@example.com",
                cancellationToken);

            return Ok(new ApiResponse<DocumentDetailsResponse>
            {
                Success = true,
                Message = "Document metadata updated successfully",
                Data = document
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<DocumentDetailsResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document metadata");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "Failed to update document metadata"
                });
        }
    }

    /// <summary>
    /// Replace a document file (admin/HR only)
    /// </summary>
    [HttpPost("{documentId}/replace-file")]
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DocumentDetailsResponse>>> ReplaceDocumentFile(
        Guid documentId,
        [FromForm] ReplaceDocumentFileForm request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Replacing file for document {DocumentId} by user {CurrentUserId}",
            documentId,
            _currentUser.EmployeeId);

        try
        {
            // Validate file
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "No file provided"
                });
            }

            if (!IsValidFileFormat(request.File.ContentType))
            {
                return BadRequest(new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "Invalid file format. Allowed formats: PDF, Word, Excel, Images"
                });
            }

            // Read file content
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream, cancellationToken);

            var replaceRequest = new ReplaceDocumentFileRequest
            {
                FileContent = memoryStream.ToArray(),
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length
            };

            var document = await _documentService.ReplaceDocumentFileAsync(
                documentId,
                replaceRequest,
                _currentUser.Email ?? "unknown@example.com",
                _currentUser.EmployeeId,
                cancellationToken);

            return Ok(new ApiResponse<DocumentDetailsResponse>
            {
                Success = true,
                Message = "Document file replaced successfully",
                Data = document
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<DocumentDetailsResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing document file");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<DocumentDetailsResponse>
                {
                    Success = false,
                    Message = "Failed to replace document file"
                });
        }
    }

    /// <summary>
    /// Delete a document (admin/HR only)
    /// </summary>
    [HttpDelete("{documentId}")]
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Deleting document {DocumentId} by user {CurrentUserId}",
            documentId,
            _currentUser.EmployeeId);

        try
        {
            await _documentService.DeleteDocumentAsync(documentId, cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<string>
                {
                    Success = false,
                    Message = "Failed to delete document"
                });
        }
    }

    /// <summary>
    /// Download a document file
    /// </summary>
    [HttpGet("{documentId}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading document {DocumentId}", documentId);

        try
        {
            var (fileContent, fileName, contentType) = 
                await _documentService.DownloadDocumentAsync(documentId, cancellationToken);

            return File(fileContent, contentType, fileName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<string>
                {
                    Success = false,
                    Message = "Failed to download document"
                });
        }
    }

    /// <summary>
    /// Validate file format
    /// </summary>
    private bool IsValidFileFormat(string contentType)
    {
        var allowedTypes = new[]
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "image/jpeg",
            "image/png",
            "image/gif"
        };

        return allowedTypes.Contains(contentType);
    }
}

public sealed class UploadDocumentForm
{
    public Guid EmployeeId { get; set; }

    public string DocumentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public IFormFile? File { get; set; }
}

public sealed class ReplaceDocumentFileForm
{
    public IFormFile? File { get; set; }
}
