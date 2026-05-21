using Mph.EFCore.Infrastructure.Common;
using Mph.EFCore.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Mph.EFCore.Infrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QRCodeController : ControllerBase
    {
        [HttpPost("generate")]
        public async Task<IActionResult> GenQRCode([FromBody][Required] GenQRCodeOptions option)
        {
            var res = HttpResult.Fail();
            try
            {
                var link = $"{option.Host}/#/scan?sid={option.Sid}&command={option.Command}";
                if (option.CommandParameters != null && option.CommandParameters.Count > 0)
                {
                    foreach (var parameter in option.CommandParameters)
                    {
                        link += $"&cp_{parameter.Key}={parameter.Value}";
                    }
                }
                if (option.MetaData != null && option.MetaData.Count > 0)
                {
                    foreach (var metadata in option.MetaData)
                    {
                        link += $"&{metadata.Key}={metadata.Value}";
                    }
                }
                res.IsSuccess = true;
                res.Code = 200;
                res.Data = link;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody][Required] ScanCodeOptions options)
        {
            var res = HttpResult.Fail();
            // 推送给目标电脑
            await WebSocketService.SendMessage(options.Sid, options);
            res.IsSuccess = true;
            res.Code = 0;
            res.Message = "指令已推送";
            return Ok(res);
        }
    }
}
