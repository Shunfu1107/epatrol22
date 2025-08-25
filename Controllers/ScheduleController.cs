using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;

namespace AdminPortalV8.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;

        public ScheduleController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
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

                var schedule = await _context.Schedules.ToListAsync();
                ViewBag.Schedules = schedule;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public ActionResult Create(Schedule model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Schedules.Add(model);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the schedule.";
                    return RedirectToAction("Index");
                }
            }

            TempData["ErrorMessage"] = "Invalid data. Please try again.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int ScheduleId, string ScheduleName, string Day, TimeOnly StartTime, TimeOnly EndTime)
        {
            var schedule = await _context.Schedules.FindAsync(ScheduleId);
            if (schedule == null)
            {
                TempData["ErrorMessage"] = "Schedule not found.";
                return RedirectToAction("Index");
            }

            schedule.ScheduleName = ScheduleName;
            schedule.Day = Day;
            schedule.StartTime = StartTime;
            schedule.EndTime = EndTime;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int scheduleId)
        {
            try
            {
                var schedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
                if (schedule != null)
                {
                    _context.Schedules.Remove(schedule);
                    _context.SaveChanges();
                }
                else
                {
                    TempData["ErrorMessage"] = "Schedule not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete the schedule. Please try again.";
            }

            return RedirectToAction("Index");
        }
    }
}