using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace TimeTracking
{
    public static class CommonFunctions
    {
        private const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public static string GetUserId(this IPrincipal user)
        {
            if (user == null && !(user is ClaimsPrincipal))
            {
                throw new Exception("invalid user");
            }

            var userIdentifier = ((ClaimsPrincipal)user).FindFirst(ObjectIdentifier);
            if(userIdentifier == null)
            {
                throw new Exception("invalid user");
            }

            return !string.IsNullOrWhiteSpace(userIdentifier.Value) ? userIdentifier.Value : throw new Exception("invalid user");
        }

        public static UserRole GetCurrentUserRole()
        {
            return UserRole.Admin;
        }
    }
}