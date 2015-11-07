using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class UserTournamentPropertyValue
    {
        [ForeignKey("User")]
        [Key, Column(Order = 0)]
        public String UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("UserProperty")]
        [Key, Column(Order = 1)]
        public Guid UserPropertyId { get; set; }

        public virtual UserProperty UserProperty { get; set; }

        [ForeignKey("Tournament")]
        [Key, Column(Order = 2)]
        public Guid TournamentId { get; set; }

        public Tournament Tournament { get; set; }
        public String Value { get; set; }

        public UserPropertyValue ToUserPropertyValue()
        {
            UserPropertyValue userPropertyValue = new UserPropertyValue
            {
                User = this.User,
                UserId = this.UserId,
                UserProperty = this.UserProperty,
                UserPropertyId = this.UserPropertyId,
                Value = this.Value
            };
            return userPropertyValue;
        }

    }
}
