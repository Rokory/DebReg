using System;
using System.Linq;

namespace DebReg.Web.APIModels
{
    public class UserPropertyValue
    {
        public String userId { get; set; }
        public Guid propertyId { get; set; }
        public Guid? tournamentId { get; set; }
        public String name { get; set; }
        public String value { get; set; }
        public String displayValue { get; set; }
        public UserPropertyValue()
        {

        }

        public UserPropertyValue(DebReg.Models.UserPropertyValue propertyValue)
            : this()
        {
            if (propertyValue != null)
            {
                this.userId = propertyValue.UserId;
                this.propertyId = propertyValue.UserPropertyId;
                this.name = propertyValue.UserProperty.Name;
                this.value = propertyValue.Value;
                this.displayValue = propertyValue.Value;

                if (propertyValue.UserProperty.Type == DebReg.Models.PropertyType.SingleSelect ||
                    propertyValue.UserProperty.Type == DebReg.Models.PropertyType.Country)
                {
                    Guid optionId;
                    if (Guid.TryParse(propertyValue.Value, out optionId))
                    {
                        var option = propertyValue.UserProperty.Options.FirstOrDefault(o =>
                            o.Id == optionId);
                        if (option != null)
                        {
                            this.displayValue = option.Name;
                        }
                    }

                }

                // TODO: Implement DisplayValue for person type

            }
        }

        public UserPropertyValue(DebReg.Models.UserTournamentPropertyValue propertyValue)
            : this()
        {
            if (propertyValue != null)
            {
                this.userId = propertyValue.UserId;
                this.propertyId = propertyValue.UserPropertyId;
                this.tournamentId = propertyValue.TournamentId;
                this.name = propertyValue.UserProperty.Name;
                this.value = propertyValue.Value;
            }
        }
    }
}