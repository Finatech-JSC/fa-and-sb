using PhoneNumbers;

namespace MicroBase.Share.Extensions
{
    public static class UserExtensions
    {
        public static string GenerateSystemEmail(this string userName)
        {
            return $"{userName}@{StaticModel.Application.Domain}";
        }

        public static string GetBlacklistKey(string accountId)
        {
            var key = "Blacklist:" + accountId;
            return key;
        }

        public static bool IsPhoneNumber(this string phoneNumber, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(countryCode))
            {
                countryCode = "SG";
            }

            var phoneUtil = PhoneNumberUtil.GetInstance();
            PhoneNumber vnPhoneNumber;
            try
            {
                vnPhoneNumber = phoneUtil.Parse(phoneNumber, countryCode);
            }
            catch (System.Exception)
            {
                return false;
            }
            
            bool isValidNumber = phoneUtil.IsValidNumber(vnPhoneNumber);
            bool isValidRegion = phoneUtil.IsValidNumberForRegion(vnPhoneNumber, countryCode);

            return isValidNumber && isValidRegion ? true : false;
        }

        public static bool IsEmailAddress(this string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }
    }
}