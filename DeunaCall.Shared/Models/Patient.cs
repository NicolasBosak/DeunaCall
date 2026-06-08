namespace DeunaCall.Shared.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string AccessCode { get; set; } = string.Empty;
}
