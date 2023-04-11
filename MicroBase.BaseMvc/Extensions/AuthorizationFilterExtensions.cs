using MicroBase.Share.Constants;
using MicroBase.Share.Models.Accounts;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MicroBase.BaseMvc.Extensions
{
    public static class AuthorizationFilterExtensions
    {
        public static void SetClaims(this AuthorizationFilterContext filterContext, LoginResponse loggedUser)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.Jwt.ClaimKeys.Id, loggedUser.Id.ToString()),
                new Claim(Constants.Jwt.ClaimKeys.UserName, loggedUser.UserName),
                new Claim(Constants.Jwt.ClaimKeys.AccountType, loggedUser.AccountType.ToString()),
                new Claim(Constants.Jwt.ClaimKeys.Via, loggedUser.Via.ToString()),
                new Claim(Constants.Jwt.ClaimKeys.IsConfirmed, loggedUser.IsConfirmed.ToString())
            };

            var appIdentity = new ClaimsIdentity(claims);
            filterContext.HttpContext.User.AddIdentity(appIdentity);
        }

        public static void SetClaims(this AuthorizationFilterContext filterContext, 
            Guid identityUserId,
            string userName,
            string accountType,
            string via,
            bool isConfirmed)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.Jwt.ClaimKeys.Id, identityUserId.ToString()),
                new Claim(Constants.Jwt.ClaimKeys.UserName, userName),
                new Claim(Constants.Jwt.ClaimKeys.AccountType, accountType),
                new Claim(Constants.Jwt.ClaimKeys.Via, via),
                new Claim(Constants.Jwt.ClaimKeys.IsConfirmed, isConfirmed.ToString())
            };

            var appIdentity = new ClaimsIdentity(claims);
            filterContext.HttpContext.User.AddIdentity(appIdentity);
        }
    }
}