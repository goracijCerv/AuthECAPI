using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace AuthECAPI.Controllers
{
    public static class AuthorizationDemoEndpoints
    {
        public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints (this IEndpointRouteBuilder app)
        {
            app.MapGet("/AdminOnly", AdminOnly);
            
            app.MapGet("/AdminOrTeacher", [Authorize(Roles = "Admin, Teacher")]  () =>
            {
                return "Admin or Teacher";
            });

            app.MapGet("/LibraryMembersOnly", [Authorize(Policy  = "HasLibraryID")] () =>
            {
                return "Library members only";
            });

            app.MapGet("/ApplyFormMaternityLeave", [Authorize(Roles = "Teacher", Policy = "FemalesOnly")] () =>
            {
                return "Applied for maternity leave";
            });

            app.Map("/Under10sAndFemale", 
            [Authorize(Policy ="Under10")]
            [Authorize(Policy = "FemalesOnly")] () =>
            {
                return "Under 10 and Female";
            });

            return app;
        }

        [Authorize(Roles ="Admin")]
        private static string AdminOnly()
        {
            return "Adim";
        }
    }
}
