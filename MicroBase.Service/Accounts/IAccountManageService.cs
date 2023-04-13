using AutoMapper;
using MicroBase.FileStorage;
using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Extensions;
using MicroBase.RedisProvider;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using MicroBase.Share.Models.CMS;
using MicroBase.Share.Models.CMS.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Transactions;
using IdentityUser = MicroBase.Entity.Accounts.IdentityUser;
using Microsoft.Extensions.Configuration;
using MicroBase.Share.Constants;

namespace MicroBase.Service.Accounts
{
    public interface IAccountManageService : IGenericService<IdentityUser, Guid>
    {
        Task<TPaging<SystemAccountResponse>> GetAdminAccountsAsync(int pageIndex, int pageSize);

        Task<AccountProfileModel> GetAccountByIdAsync(Guid identityUserId);

        Task<TPaging<IdentityUserCmsResponse>> GetIdentityUsersAsync(string searchTerm,
            DateTime? fromTime,
            DateTime? toTime,
            int pageIndex,
            int pageSize);

        Task<BaseResponse<object>> LockOrUnlockAsync(string email, bool isSystemLocked,
            DateTime? lockEndDate,
            string description);

        Task<BaseCmsResponse<object>> UpdateCMSAccountAsync(Guid id, IdentityUserResponse model);

        Task<BaseCmsResponse<object>> Remove2FAAsync(Guid accountId, Remove2FAModel model);

        Task<BaseCmsResponse<object>> LockAccountByListAsync(IEnumerable<AccountLockImportFileModel> dataFromFile,
            string description,
            string emailKeyCode);

        Task<BaseCmsResponse<object>> UnLockAccountByListAsync(IEnumerable<AccountUnLockImportFileModel> dataFromFile);

        Task<BaseResponse<object>> UpdateAccountAsync(Guid accountId, RegisterModel model);

        Task<BaseResponse<object>> UpdateExistUserAsysnc(IdentityUserCmsModel userCmsModel);

        Task<BaseResponse<object>> AddNewUserAsync(IdentityUserCmsModel userCmsModel);

        Task<BaseResponse<object>> UpdateUserAsync(IdentityUserCmsModel userCmsModel);

        Task<BaseResponse<object>> DeleteUserAsync(Guid entityId);
    }

    public class AccountManageService : GenericService<IdentityUser, Guid>, IAccountManageService
    {
        private readonly MicroDbContext microDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<AccountManageService> logger;
        private readonly IRepository<IdentityUserMetaData, Guid> userMetaDataRepo;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IRepository<IdentityUser, Guid> identityUserRepo;
        private readonly IRedisStogare redisStogare;
        private readonly IFileUploadService fileUploadService;

        public AccountManageService(IRepository<IdentityUser, Guid> repository,
            MicroDbContext microDbContext,
            ILogger<AccountManageService> logger,
            IRepository<IdentityUserMetaData, Guid> userMetaDataRepo,
            UserManager<IdentityUser> userManager,
            IRepository<IdentityUser, Guid> identityUserRepo,
            IMapper mapper,
            IConfiguration configuration,
            IRedisStogare redisStogare,
            FileUploadFactory fileUploadFactory)
            : base(repository)
        {
            this.microDbContext = microDbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.userMetaDataRepo = userMetaDataRepo;
            this.userManager = userManager;
            this.identityUserRepo = identityUserRepo;
            this.redisStogare = redisStogare;

            var uploadServiceName = configuration.GetValue<string>("FileManage:EnableService");
            this.fileUploadService = fileUploadFactory.GetServiceByName(uploadServiceName);
        }

        protected override void ApplyDefaultSort(FindOptions<IdentityUser> findOptions)
        {
            findOptions.SortDescending(s => s.CreatedDate);
        }

        public async Task<TPaging<SystemAccountResponse>> GetAdminAccountsAsync(int pageIndex, int pageSize)
        {
            var predicate = PredicateBuilder.Create<IdentityUser>(s => !s.IsDelete
                && s.AccountType == Constants.Account.Type.Operator.ToString());

            var query = await microDbContext.Set<IdentityUser>()
                .Include(s => s.IdentityUserMetaData)
                .Include(s => s.IdentityUserACGroups)
                .ThenInclude(s => s.IdentityUserRoleGroup)
                .Where(predicate)
                .OrderByDescending(s => s.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var res = new List<SystemAccountResponse>();
            foreach (var item in query)
            {
                var account = mapper.Map<SystemAccountResponse>(item);
                account.CreatedDate = account.CreatedDate.UtcToVietnamTime();
                account.ModifiedDate = account.ModifiedDate.UtcToVietnamTime();

                account.RoleGroups = new List<NameValueModel<Guid>>();

                foreach (var per in item.IdentityUserACGroups.Where(s => !s.IsDelete))
                {
                    if (per.RoleGroupId.HasValue)
                    {
                        account.RoleGroups.Add(new NameValueModel<Guid>
                        {
                            Value = per.RoleGroupId.Value,
                            Name = per.IdentityUserRoleGroup.Name
                        });
                    }
                }

                account.RoleGroupsName = string.Join(", ", account.RoleGroups.Select(s => s.Name));
                res.Add(account);
            }

            var totalRows = await Repository.CountAsync(predicate);
            return new TPaging<SystemAccountResponse>
            {
                Source = res,
                TotalRecords = totalRows
            };
        }

        public async Task<AccountProfileModel?> GetAccountByIdAsync(Guid identityUserId)
        {
            var identity = await microDbContext.Set<IdentityUser>()
                .Include(s => s.IdentityUserMetaData)
                .Include(s => s.IdentityUserTwoFAs.Where(t => !t.IsDelete))
                .FirstOrDefaultAsync(s => s.Id == identityUserId);

            if (identity == null)
            {
                return null;
            }

            return new AccountProfileModel
            {
                AccountType = identity.AccountType,
                AllowAppNotification = identity.IdentityUserMetaData != null ? identity.IdentityUserMetaData.AllowAppNotification : false,
                AllowEmailNotification = identity.IdentityUserMetaData != null ? identity.IdentityUserMetaData.AllowEmailNotification : false,
                Avatar = identity.IdentityUserMetaData?.Avatar,
                Email = identity.Email,
                DefaultLanguage = identity.IdentityUserMetaData?.DefaultLanguage,
                DateOfBirth = identity.IdentityUserMetaData?.DateOfBirth,
                EmailConfirmDate = identity.EmailConfirmDate,
                EmailConfirmed = identity.EmailConfirmed,
                Fullname = identity.FullName,
                Gender = identity.IdentityUserMetaData != null && identity.IdentityUserMetaData.Gender.HasValue
                    ? identity.IdentityUserMetaData.Gender.Value
                    : (sbyte)Constants.Account.Gender.Other,
                Id = identity.Id,
                IsDefaultPassword = identity.IsDefaultPassword,
                LatestLoginTime = identity.LastLoginTime,
                PhoneNumber = identity.PhoneNumber,
                LastLoginIpAddress = identity.LastLoginIpAddress,
                PhoneNumberConfirmed = identity.PhoneNumberConfirmed,
                UserName = identity.UserName,
                Via = identity.Via
            };
        }

        public async Task<TPaging<IdentityUserCmsResponse>> GetIdentityUsersAsync(string searchTerm,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var predicate = PredicateBuilder
                    .Create<IdentityUser>(s => !s.IsDelete && s.AccountType == Constants.Account.Type.Normal.ToString());
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    predicate = predicate.And(s => s.NormalizedUserName.Contains(searchTerm.CustomTrim().ToUpper())
                        || s.NormalizedEmail.Contains(searchTerm.CustomTrim().ToUpper())
                        || s.PhoneNumber.Contains(searchTerm.CustomTrim())
                        || s.IdentityUserMetaData.NormalizedWalletAddress.Contains(searchTerm.CustomTrim().ToUpper())
                        || s.IdentityUserMetaData.WalletAddress.Contains(searchTerm.CustomTrim()));
                }

                if (fromDate.HasValue)
                {
                    predicate = predicate.And(s => s.CreatedDate >= fromDate.Value.Date);
                }

                if (toDate.HasValue)
                {
                    predicate = predicate.And(s => s.CreatedDate < toDate.Value.Date);
                }

                var userEntities = await microDbContext.Set<IdentityUser>()
                    .Include(s => s.IdentityUserMetaData)
                    .ThenInclude(s => s.Province)
                    .Where(predicate)
                    .OrderByDescending(s => s.CreatedDate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                int totalRows = await microDbContext.Set<IdentityUser>()
                    .Include(s => s.IdentityUserMetaData)
                    .Where(predicate)
                    .CountAsync(predicate);

                var res = new List<IdentityUserCmsResponse>();
                foreach (var user in userEntities)
                {
                    res.Add(new IdentityUserCmsResponse
                    {
                        Id = user.Id,
                        Email = user.Email,
                        CreatedDate = user.CreatedDate.UtcToVietnamTime(),
                        ModifiedDate = user.ModifiedDate.UtcToVietnamTime(),
                        PhoneNumber = user.PhoneNumber,
                        PostCode = user.IdentityUserMetaData?.PostCode,
                        Address = user.IdentityUserMetaData?.Address,
                        UserName = user.UserName,
                        UserNameKana = user.UserNameKana,
                        ProvinceId = user.IdentityUserMetaData?.ProvinceId,
                        ProvinceName = user.IdentityUserMetaData?.Province?.FullName,
                    });
                }

                return new TPaging<IdentityUserCmsResponse>
                {
                    Source = res,
                    TotalRecords = totalRows
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new TPaging<IdentityUserCmsResponse>
                {
                    Source = new List<IdentityUserCmsResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<BaseResponse<object>> LockOrUnlockAsync(string email, bool isSystemLocked,
            DateTime? lockEndDate,
            string description)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND
                };
            }

            string language = string.Empty;
            var dics = new Dictionary<string, string>();

            try
            {
                var account = await microDbContext.Set<IdentityUser>()
                    .Include(s => s.IdentityUserMetaData)
                    .FirstOrDefaultAsync(s => s.NormalizedEmail == email.ToUpper());

                if (account == null)
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.NOT_FOUND
                    };
                }

                if (isSystemLocked)
                {
                    account.IsSystemLocked = true;
                    account.LockedDescription = description;
                    account.LockoutEnd = lockEndDate.HasValue ? lockEndDate.Value : DateTime.UtcNow.AddYears(100);
                }
                else
                {
                    account.IsSystemLocked = false;
                    account.LockedDescription = null;
                    account.LockoutEnd = null;
                    var key = UserExtensions.GetBlacklistKey(account.Id.ToString());
                    await redisStogare.KeyDelAsync(key);
                }

                await Repository.UpdateAsync(account);

                var res = new BaseResponse<object>
                {
                    Success = true
                };

                if (isSystemLocked)
                {
                    res.Message = CommonMessage.Account.LOCK_SUCCESSFUL;
                }
                else
                {
                    res.Message = CommonMessage.Account.UN_LOCK_SUCCESSFUL;
                }

                if (isSystemLocked && !string.IsNullOrWhiteSpace(email))
                {
                    var cultureCode = Constants.CultureCode.UnitedStates;
                    if (account.IdentityUserMetaData != null
                        && !string.IsNullOrEmpty(account.IdentityUserMetaData.DefaultLanguage)
                        && account.IdentityUserMetaData.DefaultLanguage.ToUpper() == Constants.CultureCode.UnitedStates)
                    {
                        cultureCode = Constants.CultureCode.UnitedStates;
                    }

                    string emailKeyCode = Constants.EmailTemplates.ACCOUNT_HAS_BEEN_LOCKED;
                    var token = new Dictionary<string, string>();
                    if (lockEndDate != null)
                    {
                        emailKeyCode = Constants.EmailTemplates.ACCOUNT_HAS_BEEN_LOCKED;
                        token.Add(Constants.EmailTemplates.EmailTokens.LOCK_DATE, lockEndDate.Value.ToString());
                    }

                    //var message = new EmailQueueModel
                    //{
                    //    CultureCode = cultureCode,
                    //    EmailTemplate = emailKeyCode,
                    //    EmailTokens = token,
                    //    ReceivingAddress = account.Email
                    //};

                    //await messageBusService.SendAsync(CampaignConstants.Queues.EmailMessage, message);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseCmsResponse<object>> UpdateCMSAccountAsync(Guid id, IdentityUserResponse model)
        {
            try
            {
                var validateEmail = UserExtensions.IsEmailAddress(model.Email);
                if (!validateEmail)
                {
                    return new BaseCmsResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.EMAIL_ADDRESS_INVALID,
                        MessageCode = nameof(CommonMessage.EMAIL_ADDRESS_INVALID)
                    };
                }

                var user = await GetByIdAsync(id);
                if (user == null)
                {
                    return new BaseCmsResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.RECORD_NOT_FOUND,
                        MessageCode = nameof(CommonMessage.RECORD_NOT_FOUND)
                    };
                }

                var checkExist = await CheckExistAccount(id, model.UserName, model.Email, model.IdentityUserMetaData.IdCardNumber);
                if (checkExist)
                {
                    return new BaseCmsResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.ACCOUNT_ALREADY_EXISTS,
                        MessageCode = nameof(CommonMessage.Account.ACCOUNT_ALREADY_EXISTS)
                    };
                }

                var avatarPath = string.Empty;
                if (model.IdentityUserMetaData.AvatarFromForm != null)
                {
                    var validate = FileExtensions.ValidateImageFile(model.IdentityUserMetaData.AvatarFromForm);
                    if (validate == false)
                    {
                        return new BaseCmsResponse<object>
                        {
                            Success = false,
                            Message = CommonMessage.Upload.FILE_EXTENSION_INVALID,
                            MessageCode = nameof(CommonMessage.Upload.FILE_EXTENSION_INVALID)
                        };
                    }

                    var uploadRes = await fileUploadService.UploadImageAsync(model.IdentityUserMetaData.AvatarFromForm, "user/avatar");
                    if (uploadRes.Success && uploadRes.Data != null)
                    {
                        avatarPath = uploadRes.Data.ThumbnailUrl;
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.Password) && model.Password.Equals(model.ConfirmPassword))
                {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.Password);
                }

                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var userMetaData = await microDbContext.Set<IdentityUserMetaData>()
                    .FirstOrDefaultAsync(s => s.IdentityUserId == id);

                var isAddNewMetaData = false;
                if (userMetaData == null)
                {
                    userMetaData = new IdentityUserMetaData
                    {
                        Id = Guid.NewGuid(),
                        IdentityUserId = id,
                    };

                    isAddNewMetaData = true;
                }

                userMetaData.DateOfBirth = model.IdentityUserMetaData.DateOfBirth;
                //userMetaData.IdCardNumber = model.IdentityUserMetaData.IdCardNumber;
                //userMetaData.IdCardIssueDate = model.IdentityUserMetaData.IdCardIssueDate;
                //userMetaData.IdCardIssueLocation = model.IdentityUserMetaData.IdCardIssueLocation;
                //userMetaData.HomeTownAddress = model.IdentityUserMetaData.HomeTownAddress;
                userMetaData.Avatar = avatarPath;

                if (!string.IsNullOrEmpty(model.IdentityUserMetaData.Gender))
                {
                    if (model.IdentityUserMetaData.Gender == ((int)Constants.Account.Gender.Male).ToString())
                    {
                        userMetaData.Gender = (int)Constants.Account.Gender.Male;
                    }

                    if (model.IdentityUserMetaData.Gender == ((int)Constants.Account.Gender.Female).ToString())
                    {
                        userMetaData.Gender = (int)Constants.Account.Gender.Female;
                    }

                    if (model.IdentityUserMetaData.Gender == ((int)Constants.Account.Gender.Other).ToString())
                    {
                        userMetaData.Gender = (int)Constants.Account.Gender.Other;
                    }
                }

                userMetaData.Address = model.IdentityUserMetaData.Address;
                userMetaData.ProvinceId = model.IdentityUserMetaData.ProvinceId;
                userMetaData.DistrictId = model.IdentityUserMetaData.DistrictId;

                using (var scope = new TransactionScope())
                {
                    Repository.Update(user);
                    if (isAddNewMetaData)
                    {
                        userMetaDataRepo.Insert(userMetaData);
                    }
                    else
                    {
                        userMetaDataRepo.Update(userMetaData);
                    }

                    scope.Complete();
                }

                return new BaseCmsResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.UPDATE_SUCCESS,
                    MessageCode = nameof(CommonMessage.UPDATE_SUCCESS)
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"UpdateCMSAccountAsync Exception: {ex}");
                return new BaseCmsResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR,
                    MessageCode = nameof(CommonMessage.UN_DETECTED_ERROR)
                };
            }
        }

        public async Task<BaseCmsResponse<object>> Remove2FAAsync(Guid accountId, Remove2FAModel model)
        {
            try
            {
                if (!model.Off2FAEmail && !model.Off2FAGoogle)
                {
                    return new BaseCmsResponse<object>
                    {
                        Success = false,
                        Message = CommonMessage.Account.TWO_FA_NOT_ENABLE
                    };
                }

                var list2FAService = new List<string>();
                if (model.Off2FAGoogle)
                {
                    list2FAService.Add(Constants.Account.TwoFAService.GOOGLE_AUTHENTICATOR.ToString());
                }

                if (model.Off2FAEmail)
                {
                    list2FAService.Add(Constants.Account.TwoFAService.EMAIL.ToString());
                }

                await microDbContext.Set<IdentityUserTwoFA>()
                    .Where(s => list2FAService.Contains(s.TwoFactorService) && s.IdentityUserId == accountId)
                    .DeleteFromQueryAsync();

                return new BaseCmsResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.REMOVE_2FA_SUCCESSFUL
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Remove2FAAsync Exception: {ex}");
                return new BaseCmsResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }

        public async Task<BaseResponse<object>> UpdateAccountAsync(Guid accountId, RegisterModel model)
        {
            var account = await Repository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.NOT_FOUND
                };
            }

            account.FullName = model.FullName;
            account.ModifiedDate = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password.Equals(model.RePassword))
            {
                account.PasswordHash = userManager.PasswordHasher.HashPassword(account, model.Password);
            }

            var res = await userManager.UpdateAsync(account);
            if (res.Succeeded)
            {
                return new BaseResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.UPDATE_PROFILE_SUCCESS
                };
            }

            return new BaseResponse<object>
            {
                Success = false,
                Message = CommonMessage.Account.UPDATE_PROFILE_FAILED
            };
        }

        private async Task<bool> CheckExistAccount(Guid id, string userName, string email, string idCardNumber)
        {
            var predicate = PredicateBuilder.Create<IdentityUser>(s => !s.IsDelete
                && s.AccountType == Constants.Account.Type.Normal.ToString()
                && s.Id != id);

            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.And(s => s.UserName == userName);
            }

            if (!string.IsNullOrEmpty(email))
            {
                predicate = predicate.And(s => s.Email == email);
            }

            var entity = microDbContext.Set<IdentityUser>()
                .Where(predicate);

            var checkExist = await entity.FirstOrDefaultAsync();
            return checkExist != null;
        }

        public async Task<BaseCmsResponse<object>> LockAccountByListAsync(IEnumerable<AccountLockImportFileModel> dataFromFile,
            string description,
            string emailKeyCode)
        {
            try
            {
                var lockList = new List<IdentityUser>();
                var users = await microDbContext.Set<IdentityUser>()
                    .Include(s => s.IdentityUserMetaData)
                    .Where(s => (!s.LockoutEnd.HasValue || (s.LockoutEnd.HasValue && s.LockoutEnd.Value < DateTime.UtcNow))
                        && dataFromFile.Select(d => d.Email.ToUpper()).Contains(s.NormalizedEmail))
                    .ToListAsync();

                foreach (var item in dataFromFile)
                {
                    var email = item.Email.CustomTrim();
                    var user = users.FirstOrDefault(s => s.NormalizedEmail == email.ToUpper());
                    if (user == null || (!item.LockTime.HasValue && !item.IsSystemLocked))
                    {
                        continue;
                    }

                    user.IsSystemLocked = true;
                    user.LockedDescription = description;
                    user.LockoutEnd = item.IsSystemLocked ? DateTime.UtcNow.AddYears(100) : new DateTime(item.LockTime.Value.Year, item.LockTime.Value.Month, item.LockTime.Value.Day, 23, 59, 59, DateTimeKind.Utc);

                    microDbContext.Set<IdentityUser>().AddRange(user);
                    lockList.Add(user);

                    var cultureCode = Constants.CultureCode.UnitedStates;
                    if (user.IdentityUserMetaData != null
                        && !string.IsNullOrEmpty(user.IdentityUserMetaData.DefaultLanguage)
                        && user.IdentityUserMetaData.DefaultLanguage.ToUpper() == Constants.CultureCode.UnitedStates)
                    {
                        cultureCode = Constants.CultureCode.UnitedStates;
                    }

                    var token = new Dictionary<string, string>();
                    if (!item.IsSystemLocked)
                    {
                        emailKeyCode = Constants.EmailTemplates.ACCOUNT_HAS_BEEN_LOCKED;
                        token.Add(Constants.EmailTemplates.EmailTokens.LOCK_DATE, item.LockTime.Value.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        emailKeyCode = Constants.EmailTemplates.ACCOUNT_HAS_BEEN_LOCKED_FOREVER;
                    }

                    //var message = new EmailQueueModel
                    //{
                    //    AccountId = user.Id,
                    //    CultureCode = cultureCode,
                    //    EmailTemplate = emailKeyCode,
                    //    EmailTokens = token,
                    //    ReceivingAddress = item.Email
                    //};

                    //emailList.Add(message);
                }

                if (lockList.Count() > 0)
                {
                    await identityUserRepo.UpdateManyAsync(lockList);

                    //foreach (var message in emailList)
                    //{
                    //    await messageBusService.SendAsync(CampaignConstants.Queues.EmailMessage, message);
                    //}
                }

                return new BaseCmsResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.LOCK_SUCCESSFUL
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"LockAccountByListAsync Exception: {ex}");
                return new BaseCmsResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }

        public async Task<BaseCmsResponse<object>> UnLockAccountByListAsync(IEnumerable<AccountUnLockImportFileModel> dataFromFile)
        {
            try
            {
                var users = await microDbContext.Set<IdentityUser>()
                    .Include(s => s.IdentityUserMetaData)
                    .Where(s => dataFromFile.Select(d => d.Email.ToUpper()).Contains(s.NormalizedEmail))
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.IsSystemLocked = false;
                    user.LockedDescription = string.Empty;
                    user.LockoutEnd = null;
                    var key = UserExtensions.GetBlacklistKey(user.Id.ToString());
                    await redisStogare.KeyDelAsync(key);

                }

                if (users.Count() > 0)
                {
                    await identityUserRepo.UpdateManyAsync(users);
                }

                return new BaseCmsResponse<object>
                {
                    Success = true,
                    Message = CommonMessage.Account.UNLOCK_SUCCESSFUL
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"UnLockAccountByListAsync Exception: {ex}");
                return new BaseCmsResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }

        public async Task<BaseResponse<object>> UpdateExistUserAsysnc(IdentityUserCmsModel userCmsModel)
        {
            IdentityUser entityByEmail = await Repository.FindOneAsync(s => s.NormalizedEmail == userCmsModel.Email.ToUpper());
            entityByEmail.IsDelete = false;
            entityByEmail.UserName = userCmsModel.UserName;
            entityByEmail.NormalizedUserName = userCmsModel.UserName.ToUpperInvariant();
            entityByEmail.UserNameKana = userCmsModel.UserNameKana;
            entityByEmail.PhoneNumber = userCmsModel.PhoneNumber;
            entityByEmail.CreatedDate = DateTime.UtcNow;

            var userMetaDataEntity = await userMetaDataRepo.FindOneAsync(s => s.IdentityUserId == entityByEmail.Id);
            userMetaDataEntity.IsDelete = false;
            userMetaDataEntity.ProvinceId = userCmsModel.ProvinceId;
            userMetaDataEntity.PostCode = userCmsModel.PostCode;
            userMetaDataEntity.Address = userCmsModel.Address;

            using (var scopre = new TransactionScope())
            {
                Repository.Update(entityByEmail);
                userMetaDataRepo.Update(userMetaDataEntity);

                scopre.Complete();
            }

            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL)
            };
        }

        public async Task<BaseResponse<object>> AddNewUserAsync(IdentityUserCmsModel userCmsModel)
        {
            var entityByEmail = await Repository.FindOneAsync(s => s.NormalizedEmail == userCmsModel.Email.ToUpper() && s.AccountType == Constants.Account.Type.Normal.ToString());
            if (entityByEmail != null)
            {
                if(entityByEmail.IsDelete == true)
                {
                    await UpdateExistUserAsysnc(userCmsModel);
                    return new BaseResponse<object>
                    {
                        Success = true,
                        Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                        MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL)
                    };
                }
                else
                {
                    return new BaseResponse<object>
                    {
                        Success = true,
                        Message = CommonMessage.Account.EMAIL_ALREADY_EXISTS,
                        MessageCode = nameof(CommonMessage.Account.EMAIL_ALREADY_EXISTS)
                    };
                }
            }

            var id = userCmsModel.Id.HasValue ? userCmsModel.Id.Value : Guid.NewGuid();
            var accountEntity = new IdentityUser
            {
                Id = id,
                UserName = userCmsModel.UserName,
                NormalizedUserName = userCmsModel.UserName.ToUpperInvariant(),
                UserNameKana = userCmsModel.UserNameKana,
                Email = userCmsModel.Email,
                NormalizedEmail = userCmsModel.Email.ToUpperInvariant(),
                EmailConfirmed = false,
                PhoneNumber = userCmsModel.PhoneNumber,
                PhoneNumberConfirmed = false,
                CreatedDate = DateTime.UtcNow,
                AccountType = Constants.Account.Type.Normal.ToString(),
                IsDefaultPassword = true
            };

            var result = await userManager.CreateAsync(accountEntity, Guid.NewGuid().ToString());
            if (!result.Succeeded)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.REGISTER_UN_SUCCESSFULLY,
                    MessageCode = nameof(CommonMessage.Account.REGISTER_UN_SUCCESSFULLY)
                };
            }

            var identityUserMetaDataEntity = new IdentityUserMetaData
            {
                Id = id,
                IdentityUserId = accountEntity.Id,
                ProvinceId = userCmsModel.ProvinceId,
                PostCode = userCmsModel.PostCode,
                Address = userCmsModel.Address
            };

            await userMetaDataRepo.InsertAsync(identityUserMetaDataEntity);
            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.REGISTER_SUCCESSFUL,
                MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL)
            };
        }

        public async Task<BaseResponse<object>> UpdateUserAsync(IdentityUserCmsModel userCmsModel)
        {
            var entityByEmail = await Repository
                .FindOneAsync(s => s.NormalizedEmail == userCmsModel.Email.ToUpper() && s.Id != userCmsModel.Id);
            if (entityByEmail != null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.Account.EMAIL_CMS_ALREADY_EXISTS,
                    MessageCode = nameof(CommonMessage.Account.EMAIL_CMS_ALREADY_EXISTS)
                };
            }

            var accountEntity = await Repository.GetByIdAsync(userCmsModel.Id.Value);
            accountEntity.UserName = userCmsModel.UserName.ToLowerInvariant();
            accountEntity.NormalizedUserName = userCmsModel.UserName.ToUpperInvariant();
            accountEntity.UserNameKana = userCmsModel.UserNameKana;
            accountEntity.Email = userCmsModel.Email;
            accountEntity.NormalizedEmail = userCmsModel.Email.ToUpperInvariant();
            accountEntity.PhoneNumber = userCmsModel.PhoneNumber;
            accountEntity.ModifiedDate = DateTime.UtcNow;
            accountEntity.AccountType = Constants.Account.Type.Normal.ToString();

            var identityUserMetaDataEntity = await userMetaDataRepo.FindOneAsync(s => s.IdentityUserId == accountEntity.Id);
            identityUserMetaDataEntity.ProvinceId = userCmsModel.ProvinceId;
            identityUserMetaDataEntity.PostCode = userCmsModel.PostCode;
            identityUserMetaDataEntity.Address = userCmsModel.Address;

            using (var scopre = new TransactionScope())
            {
                Repository.Update(accountEntity);
                userMetaDataRepo.Update(identityUserMetaDataEntity);

                scopre.Complete();
            }

            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.UPDATE_PROFILE_SUCCESS,
                MessageCode = nameof(CommonMessage.Account.REGISTER_SUCCESSFUL)
            };
        }

        public async Task<BaseResponse<object>> DeleteUserAsync(Guid entityId)
        {
            var accountEntity = await Repository.GetByIdAsync(entityId);
            accountEntity.IsDelete = true;

            var identityUserMetaDataEntity = await userMetaDataRepo.FindOneAsync(s => s.IdentityUserId == accountEntity.Id);
            identityUserMetaDataEntity.IsDelete = true;

            using (var scopre = new TransactionScope())
            {
                Repository.Update(accountEntity);
                userMetaDataRepo.Update(identityUserMetaDataEntity);

                scopre.Complete();
            }

            return new BaseResponse<object>
            {
                Success = true,
                Message = CommonMessage.Account.DELETE_ACCOUNT_SUCCESSFUL,
                MessageCode = nameof(CommonMessage.Account.DELETE_ACCOUNT_SUCCESSFUL)
            };
        }
    }
}