namespace Maintenance_management.domain.Entities;

public class DataEntry : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}
