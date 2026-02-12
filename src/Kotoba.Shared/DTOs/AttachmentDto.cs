using Kotoba.Domain.Enums;

namespace Kotoba.Shared.DTOs;

public class AttachmentDto
{
    public Guid AttachmentId { get; set; }
    public Guid MessageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public string FileUrl { get; set; } = string.Empty;
}

