using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;

        public ChecklistController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
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

                var checklists = await _context.CheckLists.Include(c => c.ModelNavigation).ToListAsync();
                ViewBag.Checklists = checklists;

                // Fetch AI Models and pass them to the view
                var aiModels = await _context.Aimodels.ToListAsync();
                ViewBag.AiModels = aiModels;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewChecklist(string Type, string Name, int? ModelId)
        {
            var checklist = new CheckList
            {
                Type = Type,
                CheckListName = Name,
                ModelId = (Type?.ToLower() == "manual") ? null : ModelId
            };

            // Save to database
            _context.CheckLists.Add(checklist);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditChecklist(int CheckListId, string CheckListName, string Type, int? ModelId)
        {
            if (ModelState.IsValid)
            {
                var checklist = _context.CheckLists.Find(CheckListId);

                if (checklist != null)
                {
                    checklist.Type = Type;
                    checklist.CheckListName = CheckListName;

                    if (Type?.ToLower() == "manual")
                    {
                        // Clear ModelId for manual type
                        checklist.ModelId = null;
                    }

                    else if (ModelId.HasValue && ModelId.Value > 0)
                    {
                        checklist.ModelId = ModelId.Value;
                    }

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Checklist not found.");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ChecklistId, bool forceDelete = false)
        {
            try
            {
                var checklist = await _context.CheckLists
                    .Include(cl => cl.CheckPoints) // Include related checkpoints
                    .FirstOrDefaultAsync(cl => cl.CheckListId == ChecklistId);

                if (checklist == null)
                {
                    return Json(new { success = true, message = "Checklist deleted successfully." });
                }

                // Check if the checklist is linked to any checkpoints
                var linkedCheckpoints = await _context.CheckPoints
                    .Where(cp => cp.CheckLists.Any(cl => cl.CheckListId == ChecklistId))
                    .Select(cp => cp.CheckPointName)
                    .ToListAsync();

                if (linkedCheckpoints.Any() && !forceDelete)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"This checklist is linked to the following checkpoints: {string.Join(", ", linkedCheckpoints)}.",
                        checkpointNames = linkedCheckpoints
                    });
                }

                // If forceDelete is true, remove associations from the many-to-many relationship
                if (linkedCheckpoints.Any())
                {
                    var relatedCheckpoints = await _context.CheckPoints
                        .Where(cp => cp.CheckLists.Any(cl => cl.CheckListId == ChecklistId))
                        .ToListAsync();

                    foreach (var checkpoint in relatedCheckpoints)
                    {
                        checkpoint.CheckLists.Remove(checklist);
                    }

                    await _context.SaveChangesAsync();
                }

                // Finally, remove the checklist
                _context.CheckLists.Remove(checklist);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Checklist deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to delete the checklist. Please try again." });
            }
        }
    }
}
