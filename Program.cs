using AuthECAPI;
using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppSettings(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigurateIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

#region Config. CORS //Esto es el ejemplo de como hacer regiones xd
app.ConfigureSwaggerExplorer()
    .ConfigCors()
    .AddIdentityAuthMiddlewares();
#endregion


app.MapControllers();
app.MapGroup("/api").
    MapIdentityApi<AppUser>();
//Alponer el MapGroupAntes de le la definicion de los endpoints esto hace que se pongan en ese seudofijo
app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapAuthorizationDemoEndpoints();

app.Run();

//Esta hecho de la forma minima de la api
//app.MapPost("/api/signup", async (UserManager<AppUser> userManager , [FromBody] UserRegistrationModel userRegistrationModel) =>
//{
//    AppUser user = new AppUser()
//    {
//        UserName = userRegistrationModel.Email,
//        Email = userRegistrationModel.Email,
//        FullName = userRegistrationModel.FullName,
//    };
//   var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

//    if (result.Succeeded) 
//        return Results.Ok(result);
//    else
//        return Results.BadRequest(result);
//});

//app.MapPost("/api/signin", async (UserManager<AppUser> userManager, [FromBody] LoginModel loginModel) =>
//{
//    var user = await userManager.FindByEmailAsync(loginModel.Email);
//    if(user == null)
//    {
//        return Results.BadRequest(new {message = "Email or Password is wrong."});
//    }

//    if(await userManager.CheckPasswordAsync(user, loginModel.Password))
//    {
//        var signInkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTSecret"]!));

//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject=  new ClaimsIdentity(new Claim[]
//            {
//                new Claim("UserID", user.Id.ToString())
//            }),
//            Expires = DateTime.UtcNow.AddDays(10),
//            SigningCredentials =  new SigningCredentials(signInkey, SecurityAlgorithms.HmacSha256Signature)

//        };

//        var tokenHandler = new JwtSecurityTokenHandler();
//        var securityHandler = tokenHandler.CreateToken(tokenDescriptor);
//        var token = tokenHandler.WriteToken(securityHandler);
//        return Results.Ok(new {token});
//    }
//    else
//    {
//        return Results.BadRequest();
//    }


//});



