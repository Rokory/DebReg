
using System;
using System.Web.Mvc;

namespace DebReg.Web {
    public class ModelBinderConfig {
        public static void RegisterModelBinders(ModelBinderDictionary binders) {
            // binders.Add(typeof(DateTime), new DateTimeModelBinder(GetDateFormat()));
        }

        public static String GetDateFormat() {
            return "yyyy-mm-dd";
        }

        public static String GetDateFormatJQueryUI() {
            var format = GetDateFormat();
            format = format.Replace("M", "m");
            return format;
        }
    }
}