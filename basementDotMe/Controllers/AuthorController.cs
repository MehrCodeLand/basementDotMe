using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace basementDotMe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAutrhor()
    {
        return Ok();
    }
}
