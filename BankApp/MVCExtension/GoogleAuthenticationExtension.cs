using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

namespace BankApp.MVCExtension
{
    public static class GoogleAuthenticationExtension
    {
        public static void AddGoogleAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = config["Authentication:GoogleAuth:ClientId"];
                options.ClientSecret = config["Authentication:GoogleAuth:ClientSecret"];
                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
            });
        }
    }
}
