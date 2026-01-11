using System;
using System.Collections.Generic;
using System.Text;

namespace Vibe.Test.Servcie.Enums
{
    public enum ApiReturnCode
    {
        Success = 20000,
        Created = 20100,
        No_Content = 20400,
        Bad_Request = 40000,
        Not_Found = 40400,
        General_Error = 50000,
    }
}
