using System;

namespace DeunaCall.Shared.Models;

public class CallRequest
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public RequestType Type { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
