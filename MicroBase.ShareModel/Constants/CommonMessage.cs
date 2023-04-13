namespace MicroBase.Share.Constants
{
    public static class CommonMessage
    {
        public const string RECORD_NOT_FOUND = "Record not found";
        public const string UPDATE_SUCCESS = "Successfully updated";
        public const string UPDATE_RECORD_ID_REQUIRED = "Record ID is required";
        public const string PROCESS_DATA_SUCCESS = "Process data is successfully";
        public const string UPDATE_FAILED = "Fail updated";
        public const string INSERT_SUCCESS = "Sucessfully inserted";
        public const string SAVE_IMAGE_FAIL = "Save image is failed";
        public const string INSERT_FAILED = "Insert is failed";
        public const string DELETE_SUCCESS = "Successfully deleted";
        public const string DELETE_FAILED = "Fail deleted";
        public const string DELETE_ROOT_ADMIN = "Can not detele root admin account";
        public const string DELETE_ROOT_ROLE_GROUP = "Root role can not be change";
        public const string CODE_ALREADY_EXISTS = "The code {0} already exists";
        public const string REPORT_SUCCESS = "The report has been sent";
        public const string REQUIRED_FIELD = "The field {0} can not be blank";
        public const string REQUIRED_USERNAME_FIELD = "The field user name can not be blank";
        public const string REQUIRED_PASSWORD_FIELD = "The field password can not be blank";
        public const string REQUIRED_NAME_FIELD = "The field name can not be blank";
        public const string REQUIRED_PASSWORD_WALLET_SETTING = "The field admin password can not be blank";
        public const string NO_BLANK_ALLOW = "The field {0} does not accept space";
        public const string DATA_INVALID = "The field {0} invalid";
        public const string EMAIL_ADDRESS_INVALID = "The email address invalid";
        public const string PHONE_NUMBER_INVALID = "The phone number invalid";
        public const string INPUT_PHONE_OR_EMAIL = "Please input email address or phone number";
        public const string UN_DETECTED_ERROR = "An error occurred. Please try again latter";
        public const string UN_DETECTED_ERROR_FROM_DB = "An error occurred. Please try again latter";
        public const string MODEL_STATE_INVALID = "The input data invalid";
        public const string RANGE_LENGTH = "The number of valid characters is from {2} to {1}";
        public const string MIN_LENGTH = "The field {0} must be at least {1} characters";
        public const string MAX_LENGTH = "The field {0} only allows up to {1} characters";
        public const string URL_INVALID = "The URL invalid";
        public const string MAX_FILE_LENGTH = "Only accept files with size less than or equal to {0}Mb";
        public const string INVALID_FILE_NAME = "File name cannot contain special characters";
        public const string FILE_INVALID = "The upload file invalid";
        public const string EXCEL_FILE_INVALID = "File tải lên không hợp lệ. Chỉ chấp nhận file có định dạng *.xlsx, *.xls";
        public const string CANT_UPLOAD_GIF_FILE = "Sorry! gif format is not supported";
        public const string TOO_MANY_REQUESTS = "Too many requests";
        public const string UPLOAD_LIMIT_FAILED = "Too many requests, please try again in {0} minutes";
        public const string HTTP_REQUEST_FAILED = "Connect error";
        public const string LANGUAGE_VALIDATE_FAIL = "Content contains invalid words: {0}";
        public const string REGEX_WITHOUT_SPECIAL_CHARACTER = "The field {0} does not allow entering special characters";
        public const string REGEX_NUMBER_ONLY = "Please enter numeric data";

        public const string EMAIL_TEMPLATE_NOT_FOUND = "Template not found";
        public const string NO_2FA_ENABLED = "Please select a 2-factor authentication method before using this feature";

        public const string SUBSCRIBE_FAIELD = "Subscribe failed";
        public const string SUBSCRIBE_SUCCESS = "Subscribe successfully";

        public const string REQUIRED_ROLE_SELECTED = "Please select one or more roles";
        public const string IMPORT_FILE_SUCESSFULLY = "Tải lên dữ liệu thành công";
        public const string IMPORT_FILE_SUCESSFULLY_WITH_COUNT = "Tải lên dữ liệu thành công {0} bản ghi";

        public static class Account
        {
            public const string LOGIN_FAILED = "Invalid Username or Password";
            public const string LOGIN_ACCOUNT_LOCKED_ACCESS_FAILED_COUNT = "Your account has been locked for {0} minutes. Due to entering the wrong password {1} times continuously";
            public const string LOGIN_FAILED_ACCOUNT_NOT_ALLOW = "The account has not been activated. Please activate your account.";
            public const string LOGIN_FAILED_ACCOUNT_LOCKED = "The account has been locked!";
            public const string LOGIN_SUCCESS_REQUIRED_2FA = "Please enter a two-factor password to log in";

            public const string LOGIN_SOCIAL_NOT_SUPPORTED = "Does not support login via {0}";
            public const string LOGIN_SOCIAL_VERIRY_SUCCESS = "Account verification successful";
            public const string LOGIN_SOCIAL_VERIRY_FAILED = "Account verification failed";
            public const string LOGIN_SOCIAL_FAILED = "Login unsuccessful";

            public const string LOGOUT_SUCCESS = "Sign out successful";
            public const string UN_AUTHORIZE = "Unauthorized! Please connect to your wallet";

            public const string LOGIN_SUCCESS = "Logged in successfully";
            public const string EMAIL_ALREADY_EXISTS = "This email address is already in use. If this is your account please Login.";
            public const string PHONENUMER_ALREADY_EXISTS = "This phone number is already in use. If this is your account please Login.";
            public const string WALLET_ADDRESS_ALREADY_EXISTS = "This wallet address is already in use. If this is your account please Login.";

            public const string EMAIL_CMS_ALREADY_EXISTS = "This email address is already in use";
            public const string PHONENUMER_CMS_ALREADY_EXISTS = "This phone number is already in use";
            public const string WALLET_CMS_ADDRESS_ALREADY_EXISTS = "This wallet address is already in use";
            public const string USERNAME_CMS_ALREADY_EXISTS = "This user name is already in use";
            public const string USERNAME_KANA_CMS_ALREADY_EXISTS = "This user name kana is already in use";

            public const string ACCOUNT_ALREADY_EXISTS = "This account is already in use.";
            public const string REGISTER_SUCCESS = "Successful account registration";
            public const string REGISTER_FAILED = "Account registration failed";
            public const string REGISTER_PASSWORD_LENGTH = "Password must be at least 6 characters";
            public const string NOT_FOUND = "Can't get account information";
            public const string ACCOUNT_DOES_NOT_EXISTS = "Account does not exists";
            public const string CHANGE_PASSWORD_SUCCESS = "Change password successfully. Please log in again.";
            public const string CHANGE_PASSWORD_FAILED = "Password change failed";
            public const string CHANGE_PASSWORD_OLD_PASS_INVALID = "Old password is incorrect";
            public const string CHANGE_DEFAULT_PASSWORD_FAILED = "Password change failed";
            public const string ACCOUNT_ALREADY_ACTIVE = "The account has been successfully verified. Please login";
            public const string NO_UPDATE_AFTER_KYC = "Sorry, the account has been verified. Information cannot be changed.";

            public const string LOGIN_PASSWORD_INVALID = "Login password is incorrect";

            public const string CONFIRM_EMAIL_OR_PHONE_SUCCESS = "Account verification successful";

            public const string RESET_PASSWORD_OTP_SEND_TO_EMAIL = "Verification code has been sent to your email address";
            public const string RESET_PASSWORD_OTP_SEND_TO_PHONE = "Verification code has been sent to your phone number";

            public const string UPDATE_PROFILE_SUCCESS = "Successfully updated account information";
            public const string UPDATE_PROFILE_FAILED = "Error updating account information";
            public const string RE_PASSWORD_INVALID = "Repeated password is incorrect";
            public const string REFERRAL_ID_DOES_NOT_EXISTS = "Referral code is incorrect";
            public const string REGISTER_INVALID_EXCEPTION = "Invalid username or password";
            public const string REGISTER_UN_SUCCESSFULLY = "Account registration failed";
            public const string REGISTER_WEB3_INVALID_EXCEPTION = "Registration is failed";
            public const string REGISTER_INVALID_USERNAME = "Username is email address or phone number";
            public const string REGISTER_SUCCESSFUL_NEED_CONFIRM = "Successful account registration. Please follow the instructions to activate your account";
            public const string REGISTER_SUCCESSFUL = "Successful account registration.";
            public const string UPDATE_ACCOUNT_SUCCESSFUL = "Successful account updated.";
            public const string DELETE_ACCOUNT_SUCCESSFUL = "Successful account deleted.";
            public const string REFERRAL_ID_ACCOUNT_LOCKED = "The referral code is incorrect. Referral account has been locked";
            public const string REFERRAL_INVALID = "Referral code must not contain special characters";
            public const string LOCK_SUCCESSFUL = "Account locked successfully";
            public const string UNLOCK_SUCCESSFUL = "Account unlocked successfully";
            public const string REMOVE_2FA_SUCCESSFUL = "Turn off 2-factor authentication successfully";
            public const string UN_LOCK_SUCCESSFUL = "Account unlocked successfully";
            public const string UPDATE_NOTIFICATION_SETTING_SUCCESSFUL = "Successful notification setting";

            public const string TWO_FA_NOT_ENABLE = "The account has not enabled 2-factor authentication";
            public const string TWO_FA_EMAIL_NOT_ENABLE = "The account has not enabled 2-factor authentication via Email";
            public const string TWO_FA_EMAIL_DUPLICATE = "Email already in use";

            public const string TWO_FA_GOOGLE_ALREADY_ENABLE = "Your account has two-factor authentication enabled via Google Authenticator";
            public const string TWO_FA_GOOGLE_ENABLE_SUCCESSFULLY = "Turn on 2-factor authentication via Google Authenticator successfully";
            public const string TWO_FA_GOOGLE_DISABLED_SUCCESSFULLY = "Turn off 2-factor authentication via Google Authenticator successfully";

            public const string TWO_FA_EMAIL_ALREADY_ENABLE = "Your account has enabled 2-factor authentication via Email";
            public const string TWO_FA_EMAIL_ENABLE_SUCCESSFULLY = "Turn on 2-factor authentication via Email successfully";
            public const string TWO_FA_EMAIL_DISABLED_SUCCESSFULLY = "Turn off 2-factor authentication via Email successfully";

            public const string TWO_FA_SMS_ALREADY_ENABLE = "Your account has enabled 2-factor authentication via SMS";
            public const string TWO_FA_SMS_ENABLE_SUCCESSFULLY = "Turn on 2-factor authentication via SMS successfully";
            public const string TWO_FA_SMS_DISABLED_SUCCESSFULLY = "Turn off 2-factor authentication via SMS successfully";

            public const string TWO_FA_CODE_INVALID = "Verification code is not correct";
            public const string TWO_FA_CODE_VALID = "Valid 2-factor authentication code";
            public const string TWO_FA_SESSION_EXPIRED = "Authentication session has expired";

            public const string SUBMIT_DEL_ACCOUNT_SUCCESS = "Your request to delete your account has been received. Your account will be removed from the system after {0} days";

            public const string ACCOUNT_DISABLEB = "Account Disabled";
        }

        public static class Otp
        {
            public const string SEND_OTP_SUCCESS = "Get OTP code successfully";
            public const string VERIFY_OTP_SUCCESS = "OTP authentication successful";
            public const string VERIFY_OTP_FAILED = "OTP is invalid or expired";
            public const string GET_OTP_FAILED_TOO_MANY_REQUEST = "Please wait {0} seconds for the next OTP sending";

            public enum SendTo
            {
                Phone = 1,
                Email = 2
            }
        }

        public static class Media
        {
            public const string IMAGE_EMPTY = "Image files cannot be empty";
            public const string IMAGE_TOKEN_FAIL = "Can't recognize image file";

        }

        public static class Upload
        {
            public const string FILE_EXTENSION_INVALID = "Invalid file format. Accepts only {0} formats";
            public const string FILE_SIZE_GREATER_THAN_INVALID = "ファイルサイズが必ず15MB以下です。";
            public const string FILE_UPLOAD_FAILED = "Upload file is failed";
        }

        public static class MiraigikenNFT
        {
            public const string EXCHANGE_WALLET_NOT_FOUND = "Exchange wallet is not found";
            public const string MINT_NFT_FAILED = "Mint NFT failed";
            public const string MINT_NFT_SUCCESSFULLY = "Mint NFT successfully";
            public const string MINT_NFT_DRAFT = "NFT has been saved as a draft";
            public const string MINT_NFT_FAILED_AUTO_DRAFT = "Mint NFT failed. NFT has been saved as a draft";
            public const string CARD_BRAND_ALREADY_IN_USE = "Card brand already in use";
            public const string CARD_CONDITION_ALREADY_IN_USE = "Card condition already in use";
            public const string CARD_TYPE_ALREADY_IN_USE = "Card type already in use";
            public const string UPDATE_NFT_STATUS = "Update NFT successfully";
            public const string DELETE_NFT_ISSUED = "Cannot delete NFT in Issued state";
            public const string MINT_NFT_FAILED_AUTO_DRAFT_BALANCE = "Mint NFT failed. NFT has been saved as a draft: Insufficient balance";
        }

        public static class CryptoWallet
        {
            public const string PRIVATE_KEY_ALREADY_EXISTS = "Private key already exists";
            public const string WALLET_ADDRESS_ALREADY_EXISTS = "Wallet address already exists";
        }
    }
}