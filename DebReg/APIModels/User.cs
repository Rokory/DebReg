using System;
using System.Collections.Generic;
using System.Linq;

namespace DebReg.Web.APIModels
{
    public class User
    {
        public Guid id { get; set; }
        public String firstname { get; set; }
        public String lastname { get; set; }
        public String eMail { get; set; }
        public String phoneNumber { get; set; }
        public List<UserPropertyValue> propertyValues { get; set; }

        public IDictionary<String, object> properties { get; set; }

        public User(DebReg.Models.User user)
            : this()
        {
            if (user != null)
            {
                id = Guid.Parse(user.Id);
                firstname = user.FirstName;
                lastname = user.LastName;
                eMail = user.Email;
                phoneNumber = user.PhoneNumber;

                foreach (var propertyValue in user.PropertyValues)
                {
                    propertyValues.Add(new UserPropertyValue(propertyValue));

                    var value = GetPropertyValue(propertyValue.Value, propertyValue.UserProperty);
                    properties.Add(propertyValue.UserProperty.Name, value);
                }

            }
        }

        private string GetPropertyValue(String value, DebReg.Models.UserProperty userProperty)
        {
            var returnValue = value;
            if (userProperty.Type == DebReg.Models.PropertyType.SingleSelect ||
                userProperty.Type == DebReg.Models.PropertyType.Country)
            {
                Guid optionId;
                if (Guid.TryParse(value, out optionId))
                {
                    var option = userProperty.Options.FirstOrDefault(o =>
                        o.Id == optionId);
                    if (option != null)
                    {
                        returnValue = option.Name;
                    }
                }

            }
            return returnValue;
        }

        public User(DebReg.Models.User user, Guid tournamentId)
            : this(user)
        {
            var tournamentProperties = user.TournamentPropertyValues.Where(v => v.TournamentId == tournamentId);
            foreach (var tournamentProperty in tournamentProperties)
            {
                var value = GetPropertyValue(tournamentProperty.Value, tournamentProperty.UserProperty);
                properties.Add(tournamentProperty.UserProperty.Name, value);
                propertyValues.Add(new UserPropertyValue(tournamentProperty));
            }
        }

        public User()
        {
            propertyValues = new List<UserPropertyValue>();
            properties = new Dictionary<string, object>();
        }

    }
}