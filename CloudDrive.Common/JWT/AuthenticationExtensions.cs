using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Common.JWT
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddJWTAuthentication(this IServiceCollection services, JWTOptions jwtOpt)
        {
            return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,  //验证签发者
                        ValidateAudience = true,    //验证接收者
                        ValidateLifetime = true,    //验证签名密钥
                        ValidateIssuerSigningKey = true,    //验证签名密钥
                        ValidIssuer = jwtOpt.Issuer,    //合法的签发者
                        ValidAudience = jwtOpt.Audience, //合法的接收者
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.Key))
                    };
                });
        }
    }
}
