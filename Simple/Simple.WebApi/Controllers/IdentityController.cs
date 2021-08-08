using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.WebApi.Controllers
{
    [Route("api/[controller]")]
    //[Authorize("ApiScope")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// Authorize 表示这个方法是受保护的，需要进行认证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return new JsonResult(from claim in User.Claims select new { claim.Type, claim.Value });
        }
    }
}
