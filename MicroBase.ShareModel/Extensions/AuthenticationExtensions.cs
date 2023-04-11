using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.Share.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Get accountId from claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid? GetAccountId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(m => m.Type == Jwt.ClaimKeys.Id);
            if (claim == null)
            {
                return null;
            }

            Guid.TryParse(claim.Value, out var value);
            return value;
        }

        public static string GetVia(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(m => m.Type == Jwt.ClaimKeys.Via);
            if (claim == null)
            {
                return null;
            }

            return claim.Value;
        }

        public static string GetAccountType(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(m => m.Type == Jwt.ClaimKeys.AccountType);
            if (claim == null)
            {
                return null;
            }

            return claim.Value;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(m => m.Type == Jwt.ClaimKeys.UserName)?.Value;
        }

        public static bool IsAccountConfirmed(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(m => m.Type == Jwt.ClaimKeys.IsConfirmed);
            if (claim == null)
            {
                return false;
            }

            bool.TryParse(claim.Value, out var value);
            return value;
        }

        public static string GetBearerToken(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return string.Empty;
            }

            var token = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(token))
            {
                return string.Empty;
            }

            return token.ToString().Replace("Bearer ", "");
        }

        public static string GetIpAddress(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(HttpHeaderKey.IP_ADDRESS))
            {
                return string.Empty;
            }

            var val = httpContext.Request.Headers[HttpHeaderKey.IP_ADDRESS];
            if (string.IsNullOrWhiteSpace(val))
            {
                return string.Empty;
            }

            return val.ToString();
        }

        public static string GetLocation(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(HttpHeaderKey.LOCATION))
            {
                return string.Empty;
            }

            var val = httpContext.Request.Headers[HttpHeaderKey.LOCATION];
            if (string.IsNullOrWhiteSpace(val))
            {
                return string.Empty;
            }

            return val.ToString();
        }

        public static string GetUserAgent(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(HttpHeaderKey.USER_AGENT))
            {
                return string.Empty;
            }

            var val = httpContext.Request.Headers[HttpHeaderKey.USER_AGENT];
            if (string.IsNullOrWhiteSpace(val))
            {
                return string.Empty;
            }

            return val.ToString();
        }

        public static string GetVia(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(HttpHeaderKey.VIA))
            {
                return Via.Unknow.ToString();
            }

            var val = httpContext.Request.Headers[HttpHeaderKey.VIA];
            if (string.IsNullOrWhiteSpace(val))
            {
                return Via.Web.ToString();
            }

            return int.Parse(val).ToString();
        }

        /// <summary>
        /// Get current version of app
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetAppVersion(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(HttpHeaderKey.X_APP_VERSION))
            {
                return string.Empty;
            }

            var val = httpContext.Request.Headers[HttpHeaderKey.X_APP_VERSION];
            if (string.IsNullOrWhiteSpace(val))
            {
                return string.Empty;
            }

            return val.ToString().Trim();
        }

        /// <summary>
        /// Get OS
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetAppOsPlatfrom(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(HttpHeaderKey.X_OS_PLATFORM))
            {
                return string.Empty;
            }

            var val = httpContext.Request.Headers[HttpHeaderKey.X_OS_PLATFORM];
            if (string.IsNullOrWhiteSpace(val))
            {
                return string.Empty;
            }

            return val.ToString().Trim();
        }
    }
}