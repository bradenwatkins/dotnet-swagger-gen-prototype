namespace BillingRP.Swagger
{
    /// <summary>
    /// Describes common ARM resource properties.
    /// </summary>
    public interface IArmResource
    {
        string ArmId { get; set; }

        string ArmName { get; set; }

        string ArmType { get; set; }
    }
}
