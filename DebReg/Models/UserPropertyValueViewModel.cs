using DebReg.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DebReg.Web.Models
{
    public class UserPropertyValueViewModel : IValidatableObject
    {
        public Guid UserPropertyId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Value { get; set; }
        public Boolean Required { get; set; }
        public Decimal Min { get; set; }
        public Decimal Max { get; set; }
        public Boolean TournamentSpecific { get; set; }

        public String DisplayValue { get; set; }
        public PropertyType Type { get; set; }

        private Boolean _booleanValue;

        public Boolean BooleanValue
        {
            get { return _booleanValue; }
            set { _booleanValue = value; Value = value.ToString(); }
        }

        private int _intValue;
        public int IntValue
        {
            get { return _intValue; }
            set { _intValue = value; Value = value.ToString(); }
        }

        private Decimal _decValue;

        public Decimal DecValue
        {
            get { return _decValue; }
            set { _decValue = value; Value = value.ToString(); }
        }

        private String _stringValue;

        public String StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; Value = value; }
        }

        private DateTime? _dateTimeValue;

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateTimeValue
        {
            get { return _dateTimeValue; }
            set { _dateTimeValue = value; Value = value.ToString(); }
        }

        private String _emailValue;

        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$", ErrorMessageResourceName = "ErrorEMailRegEx", ErrorMessageResourceType = typeof(Resources.Models.Strings))]
        public String EmailValue
        {
            get { return _emailValue; }
            set { _emailValue = value; Value = value; }
        }

        private String _phoneValue;

        [RegularExpression(@"^[\+][0-9]{2,3}([ -]?\d+)+$", ErrorMessageResourceName = "ErrorPhoneNumberRegEx", ErrorMessageResourceType = typeof(Resources.Models.User.Strings))]
        public String PhoneValue
        {
            get { return _phoneValue; }
            set { _phoneValue = value; Value = value; }
        }

        public Dictionary<String, String> SelectOptions { get; set; }

        private String _singleSelectValue;

        public String SingleSelectValue
        {
            get { return _singleSelectValue; }
            set
            {
                Value = value;
                _singleSelectValue = value;
            }
        }

        public void CreateCountryList(IEnumerable<Country> countries)
        {
            SelectOptions = new Dictionary<string, string>();
            countries = countries.OrderBy(c => c.ShortName);

            foreach (var country in countries)
            {
                SelectOptions.Add(country.Id.ToString(), country.ShortName);
            }
            SetDisplayValue(Value);
        }

        private Boolean SetDisplayValue(String value)
        {
            String displayValue;

            if (value == null)
            {
                DisplayValue = null;
                return true;
            }

            if (SelectOptions.TryGetValue(value, out displayValue))
            {
                DisplayValue = displayValue;
                return true;
            }
            return false;
        }


        public UserPropertyValueViewModel(UserPropertyValue userPropertyValue)
        {
            UserPropertyId = userPropertyValue.UserPropertyId;
            Name = userPropertyValue.UserProperty.Name;
            Description = userPropertyValue.UserProperty.Description;
            Value = userPropertyValue.Value;
            DisplayValue = userPropertyValue.Value;
            Type = userPropertyValue.UserProperty.Type;
            Required = userPropertyValue.UserProperty.Required;
            Min = userPropertyValue.UserProperty.Min;
            Max = userPropertyValue.UserProperty.Max;
            TournamentSpecific = userPropertyValue.UserProperty.TournamentSpecific;

            _dateTimeValue = null;


            switch (userPropertyValue.UserProperty.Type)
            {
                case PropertyType.Boolean:
                    Boolean.TryParse(Value, out _booleanValue);
                    break;
                case PropertyType.Int:
                    int.TryParse(Value, out _intValue);
                    break;
                case PropertyType.Decimal:
                    Decimal.TryParse(Value, out _decValue);
                    break;
                case PropertyType.Date:
                    DateTime tempDateTimeValue;
                    if (DateTime.TryParse(Value, out tempDateTimeValue))
                    {
                        _dateTimeValue = tempDateTimeValue;
                    }
                    break;
                case PropertyType.String:
                    _stringValue = Value;
                    break;
                case PropertyType.Text:
                    _stringValue = Value;
                    break;
                case PropertyType.PhoneNumber:
                    _stringValue = Value;
                    break;
                case PropertyType.EMail:
                    _stringValue = Value;
                    break;
                case PropertyType.SingleSelect:
                    SelectOptions = new Dictionary<string, string>();
                    if (userPropertyValue.UserProperty != null
                        && userPropertyValue.UserProperty.Options != null)
                    {
                        var orderedOptions = userPropertyValue.UserProperty.Options
                            .OrderBy(o => o != null ? o.Order : int.MaxValue);
                        foreach (var option in orderedOptions)
                        {
                            SelectOptions.Add(option.Id.ToString(), option.Name);
                        }

                    }
                    _singleSelectValue = Value;
                    SetDisplayValue(_singleSelectValue);
                    break;
                case PropertyType.SinglePerson:
                    // TODO: Implement person finder
                    break;
                case PropertyType.Country:
                    _singleSelectValue = Value;
                    break;
                default:
                    break;
            }


        }

        public UserPropertyValueViewModel()
        {

        }

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            Decimal? numericValueToValidate = null;
            String selectOptionToValidate = null;

            String memberName = "";
            String messagePrefix = String.Format("{0}: ", Name);
            if (validationContext != null && String.IsNullOrEmpty(validationContext.MemberName))
            {
                memberName = validationContext.MemberName;
                messagePrefix = string.Empty;
            }

            switch (Type)
            {
                case PropertyType.Int:
                    numericValueToValidate = IntValue;
                    break;
                case PropertyType.Decimal:
                    numericValueToValidate = DecValue;
                    break;
                case PropertyType.Country:
                    selectOptionToValidate = SingleSelectValue;
                    break;
                case PropertyType.SingleSelect:
                    selectOptionToValidate = SingleSelectValue;
                    break;
                default:
                    break;
            }

            if (Required && String.IsNullOrWhiteSpace(Value))
            {
                results.Add
                (
                    new ValidationResult
                    (
                        messagePrefix + Resources.Models.UserPropertyValueViewModel.Strings.Required,
                        new String[] { memberName }
                    )
                );
            }



            if (selectOptionToValidate != null && SelectOptions != null)
            {
                String optionDisplayValue;
                if
                (
                    SelectOptions == null
                    || !SelectOptions.TryGetValue(selectOptionToValidate, out optionDisplayValue)
                )
                {
                    results.Add
                    (
                        new ValidationResult
                        (
                            messagePrefix + Resources.Models.UserPropertyValueViewModel.Strings.NotInOptions,
                            new String[] { memberName }
                        )
                    );
                }
            }

            if (numericValueToValidate != null)
            {
                if (numericValueToValidate < Min
                    || numericValueToValidate > Max)
                {
                    results.Add
                    (
                        new ValidationResult
                        (
                            messagePrefix + String.Format
                            (
                                Resources.Models.UserPropertyValueViewModel.Strings.RangeError,
                                Min,
                                Max
                            ),
                            new String[] { memberName }
                        )
                    );
                }
            }

            return results;
        }

        #endregion
    }
}