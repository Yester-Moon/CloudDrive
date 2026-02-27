using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Common.JWT
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims, JWTOptions jWTOptions);
    }
}
