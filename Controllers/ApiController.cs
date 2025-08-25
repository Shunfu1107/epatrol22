using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;

namespace AdminPortalV8.Controllers
{
    
    public class ApiController : Controller
    {
        private readonly IGeneral _general;

        public ApiController(IGeneral general)
        {
            _general = general;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/permissions")]
        public IActionResult GetPermissions()
        {
            var permissions = _general.GetAllContentPermissions();
            return Ok(permissions);
        }

        

    }
}
