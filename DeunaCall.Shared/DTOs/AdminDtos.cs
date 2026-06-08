namespace DeunaCall.Shared.DTOs;

public class CreateNurseDto
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreatePatientDto
{
    public string Name { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
}

public class NurseListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class PatientListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string AccessCode { get; set; } = string.Empty;
}

public class MonthlyReportDto
{
    public int TotalRequests { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public double AverageResponseTimeMinutes { get; set; }
    public double FastestResponseMinutes { get; set; }
    public double SlowestResponseMinutes { get; set; }
    public List<DailyCountDto> RequestsByDay { get; set; } = new();
}

public class DailyCountDto
{
    public int Day { get; set; }
    public int Count { get; set; }
}
