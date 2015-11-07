using System;

namespace DebRegComponents.Helpers {
    static class BookingCodeHelper {
        internal static string GuidToBookingCode(Guid guid, int length = 6) {
            byte[] bytes = guid.ToByteArray();

            var decvalue = GuidToUInt(guid);

            var base20 = BaseHelper.Base20Encode(decvalue);

            // take first 6 characters
            return base20.Substring(0, 6);
        }

        private static ulong GuidToUInt(Guid guid) {
            byte[] bytes = guid.ToByteArray();
            ulong value = 0;

            for (int i = 0; i < bytes.Length; i++) {
                value = value * (uint)(Math.Pow(2, 8 * (i / 4))) + bytes[i];
            }
            return value;
        }


    }
}