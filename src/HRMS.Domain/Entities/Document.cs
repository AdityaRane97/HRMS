using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents an employee document (Offer Letter, Payslip, Tax Documents, etc.)
/// </summary>
public class Document : AggregateRoot
{
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    public string DocumentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    // Categories: OfferLetter, AppointmentLetter, Payslip, TaxDocuments, IdentityDocuments, Certificates, Policies, Other

    public string? Description { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;
    // MIME types: application/pdf, application/vnd.ms-excel, image/jpeg, etc.

    public long FileSize { get; set; }
    // File size in bytes

    public DateTime UploadDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? DocumentUrl { get; set; }
    // URL to download or preview the document

    public Guid? UploadedByUserId { get; set; }
    // User ID of who uploaded the document (admin/HR user)

    public string? UploadedByEmail { get; set; }
    // Email of the admin/HR user who uploaded

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedByEmail { get; set; }
    // Email of who made the last update
}
