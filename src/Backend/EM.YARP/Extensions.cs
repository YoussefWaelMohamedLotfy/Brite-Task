namespace EM.YARP;

public static class Extensions
{
    extension(HttpContext context)
    {
        public string BuildRedirectUrl(string? redirectUrl, bool isLogoutRequest = false)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = "/";
            }

            if (redirectUrl.StartsWith('/'))
            {
                redirectUrl =
                    context.Request.Scheme
                    + "://"
                    + context.Request.Host
                    + context.Request.PathBase
                    + redirectUrl;
            }

            if (isLogoutRequest)
            {
                redirectUrl = "/";
            }

            return redirectUrl;
        }
    }
}
