using MailKit.Net.Smtp;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Emails;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MicroBase.Service.Emails
{
    public interface IEmailService
    {
        Task<BaseResponse<object>> SendEmailAsync(string toEmail,
            string subject,
            string message,
            IReadOnlyCollection<string> emailCcs = null,
            IReadOnlyCollection<string> emailBccs = null,
            IReadOnlyCollection<FileInfo> attachments = null);

        Task<BaseResponse<object>> SendEmailsAsync(List<EmailRequest> emails);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly ISiteSettingService siteSettingService;
        private readonly IEmailFilterService emailFilterService;

        public EmailService(ISiteSettingService siteSettingService,
            ILogger<EmailService> logger,
            IEmailFilterService emailFilterService)
        {
            this.logger = logger;
            this.siteSettingService = siteSettingService;
            this.emailFilterService = emailFilterService;
        }

        public async Task<BaseResponse<object>> SendEmailAsync(string toEmail,
            string subject,
            string message,
            IReadOnlyCollection<string> emailCcs = null,
            IReadOnlyCollection<string> emailBccs = null,
            IReadOnlyCollection<FileInfo> attachments = null)
        {
            try
            {
                var isAcceptEmail = await emailFilterService.IsAcceptEmailAsync(toEmail);
                if (!isAcceptEmail)
                {
                    logger.LogError($"Email does not allow {toEmail}");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"Email does not allow"
                    };
                }

                var smtpSetting = await siteSettingService
                    .GetByKeyAsync<SmtpSettingRequest>(Constants.SiteSettings.Keys.SMTP_SETTING, Constants.SiteSettings.Fields.StringValue, false);

                if (smtpSetting == null)
                {
                    logger.LogError("SMTP setting not found");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"SMTP setting not found"
                    };
                }

                if (!smtpSetting.Enabled)
                {
                    logger.LogError("SMTP is disabled");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"SMTP is disabled"
                    };
                }

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(smtpSetting.DisplayName, smtpSetting.FromEmailAddress));

                if (!string.IsNullOrWhiteSpace(toEmail))
                {
                    mimeMessage.To.Add(new MailboxAddress("", toEmail));
                }

                mimeMessage.Subject = subject;
                var builder = new BodyBuilder { HtmlBody = message };

                if (attachments != null)
                {
                    foreach (var attachment in attachments.ToList())
                    {
                        builder.Attachments.Add(attachment.Name, attachment.GetBytes(), ContentType.Parse(attachment.Extension.GetMimeType()));
                    }
                }

                mimeMessage.Body = builder.ToMessageBody();
                if (emailCcs != null && emailCcs.Any())
                {
                    foreach (var cc in emailCcs)
                    {
                        if (!string.IsNullOrWhiteSpace(cc))
                        {
                            mimeMessage.Cc.Add(new MailboxAddress("", cc));
                        }
                    }
                }

                if (emailBccs != null && emailBccs.Any())
                {
                    foreach (var bcc in emailBccs)
                    {
                        if (!string.IsNullOrWhiteSpace(bcc))
                        {
                            mimeMessage.Bcc.Add(new MailboxAddress("", bcc));
                        }
                    }
                }

                if (!mimeMessage.To.Any() && !mimeMessage.Cc.Any() && !mimeMessage.Bcc.Any())
                {
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"No email to send"
                    };
                }

                await Task.Run(() =>
                {
                    using (var client = new SmtpClient())
                    {
                        client.Connect(smtpSetting.Host, smtpSetting.Port, smtpSetting.EnableSsl);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        //client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(smtpSetting.Account, smtpSetting.Password);

                        client.Send(mimeMessage);
                        client.Disconnect(true);

                        //logger.LogError($"Done send mail: {subject} to {toEmail}");
                    }
                });

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = $"Email sent successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Send Email error: " + ex);
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = $"Send Email error"
                };
            }
        }

        public async Task<BaseResponse<object>> SendEmailsAsync(List<EmailRequest> emails)
        {
            try
            {
                var correctEmails = new List<EmailRequest>();
                foreach (var item in emails)
                {
                    var isAcceptEmail = await emailFilterService.IsAcceptEmailAsync(item.Email);
                    if (!isAcceptEmail)
                    {
                        logger.LogError($"Email does not allow {item.Email}");
                        return new BaseResponse<object>
                        {
                            Success = false,
                            Message = $"Email does not allow"
                        };
                    }
                    else
                    {
                        correctEmails.Add(item);
                    }
                }

                if (!correctEmails.Any())
                {
                    logger.LogError("No email to send");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"No email to send"
                    };
                }

                var smtpSetting = await siteSettingService
                    .GetByKeyAsync<SmtpSettingRequest>(Constants.SiteSettings.Keys.SMTP_SETTING, Constants.SiteSettings.Fields.StringValue, false);
                if (smtpSetting == null)
                {
                    logger.LogError("SMTP setting not found");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"SMTP setting not found"
                    };
                }

                if (!smtpSetting.Enabled)
                {
                    logger.LogError("SMTP is disabled");
                    return new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"SMTP is disabled"
                    };
                }

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(smtpSetting.DisplayName, smtpSetting.FromEmailAddress));

                await Task.Run(() =>
                {
                    using (var client = new SmtpClient())
                    {
                        client.Connect(smtpSetting.Host, smtpSetting.Port, smtpSetting.EnableSsl);
                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(smtpSetting.Account, smtpSetting.Password);
                        for (int i = 0; i < correctEmails.Count; i++)
                        {
                            var email = correctEmails[i];
                            if (!string.IsNullOrWhiteSpace(email.Email))
                            {
                                mimeMessage.Bcc.Add(new MailboxAddress("", email.Email));
                            }
                            mimeMessage.Subject = email.Subject;
                            mimeMessage.Body = new TextPart("html") { Text = email.Body };
                            //logger.LogError($"Done send mail: {email.Subject} to {email.Email}");
                        }

                        client.Send(mimeMessage);
                        client.Disconnect(true);
                    }
                });

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = $"Email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = $"Send Email error"
                };
            }
        }
    }
}