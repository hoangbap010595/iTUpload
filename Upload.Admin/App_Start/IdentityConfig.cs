using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Upload.Admin.Models;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Net.Mime;
using System.Diagnostics;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Clients;
using Twilio.Http;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Upload.Admin
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);     
            //return configSendGridasync(message);
        }
        private Task configSendGridasync(IdentityMessage message)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var proxy = new WebProxy("http://proxy.hcm.fpt.vn:80");
            var client = new SendGridClient(Keys.MAILApiKey);
            var from = new EmailAddress("lchoang1995@gmail.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("lchoang1995@gmail", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            
            var response =  client.SendEmailAsync(msg);

            return response;
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            // Twilio Begin
            //TwilioClient.Init(Keys.SMSAccountIdentification, Keys.SMSAccountPassword);

            //Twilio Begin
            var Twilio = new TwilioRestClient(
              Keys.SMSAccountIdentification,
              Keys.SMSAccountPassword);

            var result = MessageResource.Create(
                to: message.Destination,
                from: Keys.SMSAccountFrom,
                body: message.Body,
                client: Twilio
            );

            // ASPSMS Begin: https://webservice.aspsms.com/aspsmsx2.asmx?WSDL
            var soapSms = new Upload.Admin.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            soapSms.SendSimpleTextSMS(
              Keys.SMSAccountIdentification,
              Keys.SMSAccountPassword,
              message.Destination,
              Keys.SMSAccountFrom,
              message.Body);
            soapSms.Close();
            return Task.FromResult(0);
        }

    }
    public static class Keys
    {
        public static string SMSAccountIdentification = "ACd7f8819318d73c29d209371fb91edae1";
        public static string SMSAccountPassword = "df22d77470ad0358c6da28fa34b45e60";
        public static string SMSAccountFrom = "+15732674542";

        public static string MAILApiKey = "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";
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Mã xác minh của bạn là: {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Mã xác minh",
                BodyFormat = "Mã xác minh của bạn là: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromHours(3)
                    };
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
