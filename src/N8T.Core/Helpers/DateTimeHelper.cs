using System;
using System.Diagnostics;
using N8T.Core.Constants;

namespace N8T.Core.Helpers
{
    public static class DateTimeHelper
    {
        [DebuggerStepThrough]
        public static DateTime NewDateTime()
        {
            return DateTimeOffset.Now.UtcDateTime;
        }

        public static string NewSystemDateTime()
        {
            return DateTimeOffset.Now.UtcDateTime.ToString(DateTimeConstant.SimpleSystem);
        }
    }
}
