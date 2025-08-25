using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;

namespace AdminPortalV8.Controllers
{
    public class ModelController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;

        public ModelController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);

                var lists = await _general.GetPermissionDefault(userId);

                userId = 0;

                var aiModel = await _context.Aimodels.ToListAsync();
                ViewBag.Aimodels = aiModel;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Detection, string Name, string Url)
        {
            var existingModel= await _context.Aimodels.FirstOrDefaultAsync(m => m.Url == Name);
            if (existingModel != null)
            {
                TempData["ErrorMessage"] = "This AI Model already exists.";
                return RedirectToAction("Index");
            }

            var aiModel = new Aimodel
            {
                Detection = Detection,
                Name = Name,
                Url = Url
            };

            _context.Aimodels.Add(aiModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int ModelId, string Detection, string Name, string Url)
        {
            var aiModel = await _context.Aimodels.FindAsync(ModelId);
            if (aiModel == null)
            {
                TempData["ErrorMessage"] = "AI Model not found.";
                return RedirectToAction("Index");
            }

            aiModel.Detection = Detection;
            aiModel.Name = Name;
            aiModel.Url = Url;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ModelId)
        {
            try
            {
                var aiModel = await _context.Aimodels.FindAsync(ModelId);
                if (aiModel == null)
                {
                    TempData["ErrorMessage"] = "AI Model not found.";
                    return RedirectToAction("Index");
                }

                _context.Aimodels.Remove(aiModel);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete the AI model. Please try again.";
            }

            return RedirectToAction("Index");
        }
    }
}
