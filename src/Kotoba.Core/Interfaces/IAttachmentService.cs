using Kotoba.Shared.DTOs;

namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for uploading and managing file attachments
/// Owner: Vinh (Reactions & Attachments)
/// </summary>
public interface IAttachmentService
{
    Task<AttachmentDto?> UploadAttachmentAsync(UploadAttachmentRequest request);
    Task<List<AttachmentDto>> GetAttachmentsAsync(Guid messageId);
}
