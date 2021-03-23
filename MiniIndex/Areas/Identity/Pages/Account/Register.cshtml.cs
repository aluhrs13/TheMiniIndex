using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Google.Cloud.RecaptchaEnterprise.V1;
using Microsoft.ApplicationInsights;

namespace MiniIndex.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly TelemetryClient _telemetry;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            TelemetryClient telemetry)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _telemetry = telemetry;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (!ReCaptchaPassed(Request.Form["g-recaptcha-response"]))
                {
                    ModelState.AddModelError(String.Empty, "Captcha failed, if you think this is a mistake please hit Feedback above and choose one of the options to reach out to us, we're getting spammed with fake registrations so we've had to add this extra security.");
                    return Page();
                }

                IdentityUser user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    string callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public bool ReCaptchaPassed(string gRecaptchaResponse)
        {
            string SiteKey = "6LegSIoaAAAAADG_Bsv_EZPPFeBhOEmXTJg5Qx9u";
            string Token = gRecaptchaResponse;
            string AssessmentName = "RegistrationAssessment";
            string ParentProject = "projects/tranquil-bison-277423";
            string RecaptchaAction = "REGISTER";

            RecaptchaEnterpriseServiceClientBuilder CredentialBuilder = new RecaptchaEnterpriseServiceClientBuilder();
            CredentialBuilder.JsonCredentials = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON_VERSION");
            RecaptchaEnterpriseServiceClient client = CredentialBuilder.Build();

            CreateAssessmentRequest createAssessmentRequest = new CreateAssessmentRequest()
            {
                Assessment = new Assessment()
                {
                    Event = new Event()
                    {
                        SiteKey = SiteKey,
                        Token = Token,
                        ExpectedAction = RecaptchaAction
                    },
                    Name = AssessmentName,
                },
                Parent = ParentProject
            };

            Assessment response = client.CreateAssessment(createAssessmentRequest);

            if (response.TokenProperties.Valid == false)
            {
                System.Console.WriteLine("The CreateAssessment() call failed " +
                    "because the token was invalid for the following reason: " +
                    response.TokenProperties.InvalidReason.ToString());

                _telemetry.TrackEvent("ReCaptchaCompleted", new Dictionary<string, string> { { "RiskAnalysis", "-1" } });

                return false;
            }
            else
            {
                if (response.Event.ExpectedAction == RecaptchaAction)
                {
                    System.Console.WriteLine("The reCAPTCHA score for this token is: " +
                        response.RiskAnalysis.Score.ToString());

                    _telemetry.TrackEvent("ReCaptchaCompleted", new Dictionary<string, string> { { "RiskAnalysis", response.RiskAnalysis.Score.ToString() } });
                    if(response.RiskAnalysis.Score > .4)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    System.Console.WriteLine("The action attribute in your reCAPTCHA " +
                        "tag does not match the action you are expecting to score");

                    _telemetry.TrackEvent("ReCaptchaCompleted", new Dictionary<string, string> { { "RiskAnalysis", "-1" } });

                    return false;
                }

            }
        }
    }
}
