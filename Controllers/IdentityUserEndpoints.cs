using AuthECAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AuthECAPI.Controllers.IdentityUserEndpoints;

namespace AuthECAPI.Controllers
{
    public static class IdentityUserEndpoints
    {
        public class UserRegistrationModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Gender {  get; set; }
            public int Age { get; set; }
            public int? LibraryID { get; set; }
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app) {
            app.MapPost("/signup", CreateUser);

            app.MapPost("/signin", Sigin);
            return app;
        }

        [AllowAnonymous]
        private static async  Task<IResult> CreateUser(UserManager<AppUser> userManager, [FromBody] UserRegistrationModel userRegistrationModel)
        {
            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                Gender = userRegistrationModel.Gender,
                DDB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegistrationModel.Age)),
                LibraryID = userRegistrationModel.LibraryID,
                
            };
            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
            await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
            if (result.Succeeded)
                return Results.Ok(result);
            else
                return Results.BadRequest(result);
        }

        [AllowAnonymous]
        private static async Task<IResult> Sigin(UserManager<AppUser> userManager, [FromBody] LoginModel loginModel, IOptions<AppSettings> appSetings)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "Email or Password is wrong." });
            }

            if (await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var signInkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSetings.Value.JWTSecret ));

                ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
                {
                        new Claim("userID", user.Id.ToString()),
                        new Claim("gender", user.Gender.ToString()),
                        new Claim("age", (DateTime.Now.Year- user.DDB.Year).ToString()),
                        new Claim(ClaimTypes.Role, roles.First())

                });

                if(user.LibraryID != null)
                {
                    claims.AddClaim(new Claim("libraryID", user.LibraryID.ToString()!));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims ,
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(signInkey, SecurityAlgorithms.HmacSha256Signature)

                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityHandler = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityHandler);
                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest();
            }
        }

    }
}
