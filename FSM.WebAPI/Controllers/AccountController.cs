using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using FSM.WebAPI.Models;
using FSM.WebAPI.Providers;
using FSM.WebAPI.Results;
using FSM.WebAPI.Common;
using Microsoft.Practices.Unity;
using FSM.Core.Interface;
using FSM.Infrastructure;
using FSM.Core.Entities;
using System.Web.Mvc;
using System.Linq;
using log4net;

namespace FSM.WebAPI.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Account")]

    public class AccountController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public AccountController()
        {
        }
        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        [System.Web.Http.AllowAnonymous]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("ForgotPassword")]
        public async Task<ServiceResponse<string>> ForgotPassword(dynamic json)
        {
            ServiceResponse<string> resultSet = new ServiceResponse<string>();
            try
            {
                string email = json.Email.Value;
                ApplicationUser signedUser = UserManager.FindByEmail(email);
                EmployeeDetail empDetail = db.EmployeeDetail.Where(m => m.Email == email && m.IsDelete == false && m.IsActive == true).FirstOrDefault();
                if (signedUser == null || empDetail == null)
                {
                    resultSet.ResponseList = new List<string> { "Email address does not exists." };
                    resultSet.ResponseCode = 2;
                    resultSet.ResponseErrorMessage = null;
                }
                string code = await UserManager.GeneratePasswordResetTokenAsync(signedUser.Id);
                string newPassword = RandomPassword.Generate(8, 10);
                var result = await UserManager.ResetPasswordAsync(signedUser.Id, code, newPassword);

                SendEmail objSendEmail = new SendEmail();
                string body = "Hi " + signedUser.UserName + ", <br/><br/> Your New Password is <b>" + newPassword + "</b>.<br/><br/>Thanks<br/>Admin";
                objSendEmail.sendEmailViaWebApi("New Password", body, "tsingh@seasiainfotech.com", email);
                // If we got this far, something failed, redisplay form
                if (result.Succeeded)
                {
                    resultSet.ResponseList = new List<string> { "New password has been sent to the mail" };
                    resultSet.ResponseCode = 2;
                    resultSet.ResponseErrorMessage = null;
                }
                return resultSet;
            }
            catch (Exception ex)
            {
                resultSet.ResponseCode = 0;
                resultSet.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return resultSet;
            }
        }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("UserInfo")]
        public ServiceResponse<UserInfoViewModel> GetUserInfo()
        {
            ServiceResponse<UserInfoViewModel> result = new ServiceResponse<UserInfoViewModel>();
            try
            {
                ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
                result.ResponseList = new List<UserInfoViewModel>();

                result.ResponseList.Add(new UserInfoViewModel
                {
                    UserName = User.Identity.GetUserName(),
                    HasRegistered = externalLogin == null,
                    UserId = User.Identity.GetUserId()
                });

                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        public ServiceResponse<string> SaveDeviceToken(DeviceTokenViewModel deviceTokenViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                var IsExist = db.UserDeviceTokenMappings.Where(m => m.UserId == deviceTokenViewModel.UserId &&
                              m.DeviceId == deviceTokenViewModel.DeviceId).FirstOrDefault();

                CommonMapper<DeviceTokenViewModel, UserDeviceTokenMapping> mapper = new CommonMapper<DeviceTokenViewModel, UserDeviceTokenMapping>();
                UserDeviceTokenMapping userDeviceTokenMapping = mapper.Mapper(deviceTokenViewModel);

                if (IsExist != null)
                {
                    UserDeviceTokenMapping userDeviceTokenEntity = db.UserDeviceTokenMappings.Find(IsExist.Id);
                    userDeviceTokenEntity.DeviceToken = deviceTokenViewModel.DeviceToken;
                    userDeviceTokenEntity.ModifiedDate = DateTime.Now;
                    userDeviceTokenEntity.ModifiedBy = deviceTokenViewModel.UserId;

                    db.SaveChanges();
                    result.ResponseList = new List<string> { "Record updated successfully" };
                }
                else
                {
                    userDeviceTokenMapping.Id = Guid.NewGuid();
                    userDeviceTokenMapping.CreatedDate = DateTime.Now;
                    userDeviceTokenMapping.CreatedBy = deviceTokenViewModel.UserId;

                    db.UserDeviceTokenMappings.Add(userDeviceTokenMapping);
                    db.SaveChanges();
                    result.ResponseList = new List<string> { "Record saved successfully" };
                }

                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
            return result;
        }

        // POST api/Account/ChangePassword
        [System.Web.Http.Route("ChangePassword")]
        public async Task<ServiceResponse<string>> ChangePassword(ChangePasswordBindingModel model)
        {
            ServiceResponse<string> resultSet = new ServiceResponse<string>();
            try
            {
                IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    resultSet.ResponseList = new List<string> { "Password changed successfully" };
                    resultSet.ResponseCode = 1;
                    resultSet.ResponseErrorMessage = null;
                }
                else
                {
                    resultSet.ResponseList = new List<string> { "Old Password is not correct" };
                    resultSet.ResponseCode = 2;
                    resultSet.ResponseErrorMessage = null;
                }
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " change password.");

                return resultSet;
            }
            catch (Exception ex)
            {
                resultSet.ResponseCode = 0;
                resultSet.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return resultSet;
            }
        }

        // POST api/Account/Logout
        [System.Web.Http.Route("Logout")]
        public ServiceResponse<string> Logout()
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                result.ResponseList = new List<string> { "Logout successfully" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " logout " + userName);

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [System.Web.Http.Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " Manage Info");

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }



        // POST api/Account/SetPassword
        [System.Web.Http.Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " set password");

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [System.Web.Http.Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " add external login");

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [System.Web.Http.Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // GET api/Account/ExternalLogin
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }
            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/RegisterExternal
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }
                return BadRequest(ModelState);
            }
            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
        #endregion
    }
}
