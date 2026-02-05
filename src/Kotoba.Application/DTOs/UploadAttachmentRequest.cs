using Microsoft.AspNetCore.Http;

namespace Kotoba.Application.DTOs;

public class UploadAttachmentRequest
{
    public Guid MessageId { get; set; }
    public IFormFile File { get; set; } = null!;
}

