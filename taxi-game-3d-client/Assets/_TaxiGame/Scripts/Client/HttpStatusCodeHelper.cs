using System.Net;

namespace TaxiGame3D
{
    public static class HttpStatusCodeHelper
    {
        public static bool IsSuccess(this HttpStatusCode code)
        {
            var num = (int)code;
            return num >= 200 && num < 300;
        }
    }
}