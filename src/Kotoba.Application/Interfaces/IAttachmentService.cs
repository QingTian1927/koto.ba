using Kotoba.Shared.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for uploading and managing file attachments
/// Owner: Vinh (Reactions & Attachments)
/// </summary>
public interface IAttachmentService
{
    Task<AttachmentDto?> UploadAttachmentAsync(UploadAttachmentRequest request);
    Task<List<AttachmentDto>> GetAttachmentsAsync(Guid messageId);
}

