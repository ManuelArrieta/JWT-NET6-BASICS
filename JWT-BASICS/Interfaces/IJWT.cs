using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace JWT_BASICS.Interfaces
{
    public interface IJWT
    {
        public string GenerateToken();
        public string GenerateToken(Dictionary<string, object>? claimList);
    }
}
