namespace BalanceBeam.Identity.Common.Helpers.OTL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class OTLHelper
    {
        public static string ActivitySource => Assembly.GetEntryAssembly().GetName().Name;

        public const string AttributeExceptionType = "exception.type";
        public const string AttributeExceptionMessage = "exception.message";
        public const string AttributeExceptionStacktrace = "exception.stacktrace";

        public const string AttributeUserName = "user.name";
        public const string AttributeUserEmail = "user.email";
        public const string AttributeUserId = "user.id";

        public const string AttributeRegisterFailed = "user.registration.fail";
        public const string AttributeUpdateFailed = "user.update.fail";
    }
}
