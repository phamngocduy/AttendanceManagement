using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections;
using LoginManagement.Models;
using LoginManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Web.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing;

namespace IdentificationManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        static Hashtable Hashtable;
		cap21t4Entities db = new cap21t4Entities();
		static User user;

		static AccountController()
        {
            Hashtable = new Hashtable();
			user  = new User();
		}
		[AllowAnonymous]
		public ActionResult Help()
		{
			return View();
		}
		[AllowAnonymous]
		public ActionResult Index()
		{
		
			user = db.Users.First(x => x.Email == "duykhau1@vanlanguni.vn");
			return View(user);
		}
		[AllowAnonymous]
		public ActionResult Edit(string id)
		{
			int userID = int.Parse(id);
			var user = db.Users.FirstOrDefault(x => x.ID == userID);
			return View(user);
		}
		[AllowAnonymous]
		[HttpPost]
		public ActionResult Edit(User user)
		{

			User nUser = db.Users.FirstOrDefault(x => x.ID == user.ID);
			if (ModelState.IsValid)
			{
				nUser.DoB = user.DoB;
				
				nUser.PhoneNumber = user.PhoneNumber;
				db.SaveChanges();
			}
			return RedirectToAction("Index");
			
		}
		/// <summary>
		/// Avatar Controler
		/// </summary>

		private const string _tempFolder = "/Temp";
		private const string _mapTempFolder = "~" + _tempFolder;
		private  string _avatarPath = "~/App_Data/Avatars";

		private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };
		[AllowAnonymous]
		[HttpGet]
		public ActionResult Upload()
		{
			return View();
		}
		[AllowAnonymous]
		[HttpGet]
		public ActionResult _Upload()
		{
			return PartialView();
		}
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult _Upload(IEnumerable<HttpPostedFileBase> files)
		{
			if (files == null || !files.Any())
				return Json(new { success = false, errorMessage = "No file uploaded." });

			var file = files.FirstOrDefault();  // get ONE only
			if (file == null || !IsImage(file))
				return Json(new { success = false, errorMessage = "File is of wrong format." });

			if (file.ContentLength <= 0)
				return Json(new { success = false, errorMessage = "File cannot be zero length." });

			var webPath = GetTempSavedFilePath(file);

			return Json(new { success = true, fileName = webPath.Replace("\\", "/") }); // success
		}
		[AllowAnonymous]
		[HttpPost]
		public async Task<ActionResult> Save(string fileName)
		{
			try
			{
				// Get file from temporary folder, ...
				var fn = Path.Combine(Server.MapPath(_mapTempFolder), Path.GetFileName(fileName));

				// ... get the image, ...
				var img = new WebImage(fn);

				// ... scale it, ...
				img.Resize(256, 256);

				// ... delete the temporary file,...
				System.IO.File.Delete(fn);

				_avatarPath = _avatarPath + "/Avatars" + user.StID.ToString();
				// ... and save the new one.
				var newFileName = Path.Combine(_avatarPath, Path.GetFileName(fn));
				var newFileLocation = HttpContext.Server.MapPath(newFileName);
				if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
				}
				
				img.Save(newFileLocation);
				user.Avatar = newFileName;


				string imageBase64 = ImageToBase64(newFileLocation);

				return Json(new { success = true, avatarFileLocation = newFileName });				
			}
			catch (Exception ex)
			{
				return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: " + ex.Message });
			}
		}
		
	
	[AllowAnonymous]
		private bool IsImage(HttpPostedFileBase file)
		{
			if (file == null) return false;
			return file.ContentType.Contains("image") ||
				_imageFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
		}
		[AllowAnonymous]
		private string GetTempSavedFilePath(HttpPostedFileBase file)
		{
			// Define destination
			var serverPath = HttpContext.Server.MapPath(_tempFolder);
			if (Directory.Exists(serverPath) == false)
			{
				Directory.CreateDirectory(serverPath);
			}

			// Generate unique file name
			var fileName = Path.GetFileName(file.FileName);
			fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

			// Clean up old files after every save
			CleanUpTempFolder(1);
			return Path.Combine(_tempFolder, fileName);
		}
		[AllowAnonymous]
		private static string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
		{
			var img = new WebImage(file.InputStream);
			var ratio = img.Height / (double)img.Width;
			img.Resize(256, 256);

			var fullFileName = Path.Combine(serverPath, fileName);
			if (System.IO.File.Exists(fullFileName))
			{
				System.IO.File.Delete(fullFileName);
			}

			img.Save(fullFileName);
			return Path.GetFileName(img.FileName);
		}

		private void CleanUpTempFolder(int hoursOld)
		{
			try
			{
				var currentUtcNow = DateTime.UtcNow;
				var serverPath = HttpContext.Server.MapPath("/Temp");
				if (!Directory.Exists(serverPath)) return;
				var fileEntries = Directory.GetFiles(serverPath);
				foreach (var fileEntry in fileEntries)
				{
					var fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
					var res = currentUtcNow - fileCreationTime;
					if (res.TotalHours > hoursOld)
					{
						System.IO.File.Delete(fileEntry);
					}
				}
			}
			catch
			{
				// Deliberately empty.
			}
		}


		//Convert image to base64
		[AllowAnonymous]
		public string ImageToBase64(string imagepath)
		{
			using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagepath))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					string base64String;
					image.Save(ms, image.RawFormat);
					byte[] imageBytes = ms.ToArray();
					base64String = Convert.ToBase64String(imageBytes);
					return base64String;
				}
			}
		}
	
		//Convert base64 to image
		public System.Drawing.Image Base64ToImage(string base64String)
		{
			byte[] imageBytes = Convert.FromBase64String(base64String);
			MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
			ms.Write(imageBytes, 0, imageBytes.Length);
			System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
			return image;
		}

		[AllowAnonymous]
		public HttpResponseMessage getStatus()
		{
			try
			{
				var response = new HttpResponseMessage(HttpStatusCode.OK);
				response.Content = new StringContent(JsonConvert.SerializeObject(new { status = "Sucess" }));
				response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				return response;
			}
			catch
			{
				return new HttpResponseMessage(HttpStatusCode.BadRequest);
			}
		}

		[AllowAnonymous]
		public ActionResult LoginVL(string redirect_uri, string state)
		{
			if (state != null)
				Hashtable[state] = redirect_uri;
			ViewBag.ReturnUrl = state;
			string provider = "Microsoft";
			string returnUrl = state;
			//return View("Login");
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}


		[AllowAnonymous, HttpPost]
        public JsonResult GetInfo(string code)
        {
            var loginInfo = Hashtable[code] as ExternalLoginInfo;
            if (loginInfo != null)
                return Json(new
                {
                    access_token = code,
                    Email = loginInfo.Email,
                    DefaultUserName = loginInfo.DefaultUserName
                }, JsonRequestBehavior.DenyGet);
            else return null;
        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;



        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }



        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            ApplicationUser user = null;
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (returnUrl != null && Hashtable[returnUrl] != null)
                    {
                        var state = returnUrl;
                        var redirect_uri = Hashtable[state];
                        user = await UserManager.FindByEmailAsync(loginInfo.Email);
                        Hashtable[user.Id] = loginInfo;
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return Redirect(string.Format("{0}?state={1}&code={2}", redirect_uri, state, user.Id));
                    }
                    else return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    user = new ApplicationUser { UserName = loginInfo.Email, Email = loginInfo.Email };
                    if (UserManager.Create(user).Succeeded)
                    {
                        if (UserManager.AddLogin(user.Id, loginInfo.Login).Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            goto case SignInStatus.Success;
                        }
                    }
                    // If the user does not have an account, then prompt the user to create an account
                    //ViewBag.ReturnUrl = returnUrl;
                    //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
           if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}