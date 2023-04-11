using MicroBase.Share.Constants;
using MicroBase.Share.Models.Accounts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MicroBase.Service.Accounts
{
    public interface IJwtService
    {
        bool ValidateToken(string authToken);

        string GenerateRefreshToken();

        JwtResponse BuildToken(IEnumerable<Claim> claims, double expireMinutes);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        UserTokenModel ValidateJwtToken(string token);

        public string GetUrlFromToken(string token, string myClaim);
    }

    public class JwtService : IJwtService
    {
        private readonly double ExpiryMinutes = 0;
        private readonly string SecretKey = string.Empty;
        private readonly string Issuer = string.Empty;
        private readonly IConfiguration configuration;
        private readonly ILogger<JwtService> logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            double.TryParse(configuration.GetValue<string>("JWT:ExpiryMinutes"), out ExpiryMinutes);
            SecretKey = configuration.GetValue<string>("JWT:SecretKey");
            Issuer = configuration.GetValue<string>("JWT:Issuer");

            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Build a token from claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public JwtResponse BuildToken(IEnumerable<Claim> claims, double expireMinutes)
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                return new JwtResponse
                {
                    Token = string.Empty
                };
            }

            var currentTime = DateTime.UtcNow;
            var expiredTime = DateTime.UtcNow.AddMinutes(expireMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(Issuer,
                Issuer,
                claims,
                currentTime,
                expiredTime,
                creds);

            return new JwtResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                ExpiredInSecond = expireMinutes
            };
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public bool ValidateToken(string authToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)) // The same key as the one that generate the token
                };

                var principal = tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Generating a 32 byte long random number and converting it to base64
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Get principal from expired token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
                    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        public UserTokenModel ValidateJwtToken(string token)
        {
            var isValid = ValidateToken(token);
            if (!isValid)
            {
                return new UserTokenModel
                {
                    IsTokenValid = isValid,
                    UserInfo = null
                };
            }

            Guid? accountId = null;
            string userName = string.Empty,
                accountType = string.Empty,
                via = string.Empty;
            bool isConfirmed = false;

            var handler = new JwtSecurityTokenHandler();
            var tokenData = handler.ReadJwtToken(token);
            if (tokenData == null || !tokenData.Claims.Any())
            {
                return new UserTokenModel
                {
                    IsTokenValid = false,
                    UserInfo = null
                };
            }

            var accountClaim = tokenData.Claims.FirstOrDefault(s => s.Type == Constants.Jwt.ClaimKeys.Id);
            if (accountClaim != null)
            {
                accountId = Guid.Parse(accountClaim.Value.ToString());
            }

            var userClaim = tokenData.Claims.FirstOrDefault(s => s.Type == Constants.Jwt.ClaimKeys.UserName);
            if (userClaim != null)
            {
                userName = userClaim.Value.ToString();
            }

            var accountTypeClaim = tokenData.Claims.FirstOrDefault(s => s.Type == Constants.Jwt.ClaimKeys.AccountType);
            if (accountTypeClaim != null)
            {
                accountType = accountTypeClaim.Value.ToString();
            }

            var viaClaim = tokenData.Claims.FirstOrDefault(s => s.Type == Constants.Jwt.ClaimKeys.Via);
            if (viaClaim != null)
            {
                via = viaClaim.Value.ToString();
            }

            var isConfirmedClaim = tokenData.Claims.FirstOrDefault(s => s.Type == Constants.Jwt.ClaimKeys.IsConfirmed);
            if (isConfirmedClaim != null)
            {
                isConfirmed = bool.Parse(isConfirmedClaim.Value.ToString());
            }

            return new UserTokenModel
            {
                IsTokenValid = true,
                UserInfo = new LoginResponse
                {
                    Id = accountId.Value,
                    UserName = userName,
                    AccountType = accountType,
                    Via = via,
                    IsConfirmed = isConfirmed
                }
            };
        }

        public string GetUrlFromToken(string token, string myClaim)
        {
            var validateImage = Validate(token);
            if (!validateImage)
            {
                return string.Empty;
            }
            try
            {
                var link = GetClaim(token, myClaim);
                return link;
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool Validate(string token)
        {
            var mySecret = configuration.GetValue<string>("JwtToken:SecretKey");
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        private string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
            return stringClaimValue;
        }
    }
}