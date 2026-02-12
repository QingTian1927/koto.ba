namespace Kotoba.Shared.DTOs;

public class CreateGroupRequest
{
    public string GroupName { get; set; } = string.Empty;
    public List<string> ParticipantIds { get; set; } = new();
}

