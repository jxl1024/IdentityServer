using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
           new ApiScope
           {
               Name="simple_api",
               DisplayName="Simple_API"
           }
        };

        public static IEnumerable<Client> Clients => new[]
        {
           new Client
           {
               ClientId = "simple_client",
               // 一个id可以对应多个秘钥
               ClientSecrets =
               {
                   // Sha256加密
                   new Secret("simple_client_secret".Sha256())
               },
               // 指定客户端授权类型  这里是客户端凭据许可模式
               AllowedGrantTypes = GrantTypes.ClientCredentials,
               // 指定API允许的范围  是个数组，可以定义多组
               // 表示当前客户端，允许访问simple_api范围里的API
               AllowedScopes={ "simple_api" }
           }
        };
    }
}
