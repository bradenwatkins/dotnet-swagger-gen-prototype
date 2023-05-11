
using Swashbuckle.AspNetCore.Annotations;

namespace BillingRP.Swagger
{
    [SwaggerSchema("Common fields that are returned in the response for all Azure Resource Manager resources.")]
    public class ArmResource<TProperties>
        where TProperties : class, IArmResource, new()
    {
        [SwaggerSchema(
            "Fully qualified resource ID for the resource. Ex - /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{resourceType}/{resourceName}.",
            ReadOnly = true)]
        public string Id
        {
            get => Properties?.ArmId;
            set
            {
                if (Properties != null)
                {
                    Properties.ArmId = value;
                }
            }
        }

        [SwaggerSchema(
            "The name of the resource.",
            ReadOnly = true)]
        public string Name
        {
            get => Properties?.ArmName;
            set
            {
                if (Properties != null)
                {
                    Properties.ArmName = value;
                }
            }
        }

        [SwaggerSchema(
            "The type of the resource. E.g. \"Microsoft.Compute/virtualMachines\" or \"Microsoft.Storage/storageAccounts\".")]
        public TProperties Properties { get; set; }

        [SwaggerSchema(
            "Resource type.",
            ReadOnly = true)]
        public string Type
        {
            get => Properties?.ArmType;
            set
            {
                if (Properties != null)
                {
                    Properties.ArmType = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmResource{TProperties}"/> class.
        /// </summary>
        public ArmResource()
            : this(new TProperties())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmResource{TProperties}"/> class.
        /// </summary>
        /// <param name="properties"></param>
        public ArmResource(TProperties properties) => Properties = properties;
    }
}
