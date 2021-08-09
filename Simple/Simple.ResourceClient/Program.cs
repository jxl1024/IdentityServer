using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Simple.ResourceClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            // 获取发现文档   获取各种配置信息 参数是基地址
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            // 判断有没有获取成功
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            //  请求资源拥有者凭据许可的token
            var tokenResponse = await client.RequestPasswordTokenAsync(
                // 参数
                new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "simple_pass_client",
                    ClientSecret = "simple_client_secret",
                    UserName="admin",
                    Password="123"
                });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            // 打印令牌信息
            Console.WriteLine(tokenResponse.Json);

            // 创建访问API的HttpClient
            var apiClient = new HttpClient();
            // 添加token
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await apiClient.GetAsync("https://localhost:6001/api/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }
            // 输出信息
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
            Console.ReadKey();
        }
    }
}
