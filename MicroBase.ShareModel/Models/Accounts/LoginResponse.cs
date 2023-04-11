using System;

namespace MicroBase.Share.Models.Accounts
{
    public interface IJwtTokenResponse
    {
        string Token { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        double ExpiredInSecond { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        DateTime ValidateTo { get; set; }
    }

    public class LoginResponse : AccountProfileModel, IJwtTokenResponse
    {
        public string SessionId { get; set; }

        public bool IsAddNewAccount { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public double ExpiredInSecond { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public DateTime ValidateTo { get; set; }
    }

    public class UserTokenModel
    {
        public bool IsTokenValid { get; set; }

        public LoginResponse UserInfo { get; set; }
    }

    public class JwtTokenResponse : IJwtTokenResponse
    {
        public string Token { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public double ExpiredInSecond { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public DateTime ValidateTo { get; set; }
    }
}