using System;
using System.Globalization;
using System.Web.Mvc;

namespace DebReg.Web.Infrastructure {
    public class DateTimeModelBinder : DefaultModelBinder {
        String customFormat;
        public DateTimeModelBinder(String dateFormat) {
            customFormat = dateFormat;
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return DateTime.ParseExact(value.AttemptedValue, customFormat, CultureInfo.InvariantCulture);
        }
    }
}