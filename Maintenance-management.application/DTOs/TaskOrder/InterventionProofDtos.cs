namespace Maintenance_management.application.DTOs.TaskOrder;

public class SubmitInterventionProofDto
{
    public double? ArrivalLatitude { get; set; }
    public double? ArrivalLongitude { get; set; }
    public string? ProofPhotoUrl { get; set; }
    public string? CustomerSignatureUrl { get; set; }
}
