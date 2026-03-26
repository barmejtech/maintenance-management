namespace Maintenance_management.application.DTOs.DataEntry;

public class DataEntryDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDataEntryDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateDataEntryDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}
