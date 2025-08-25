using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Controllers
{
    public class CheckpointsController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        public CheckpointsController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);
                userId = 0;

                var checkpoints = await _context.CheckPoints
                                    .Include(cp => cp.Location)
                                    .Include(cp => cp.CheckLists)
                                    .Include(cp => cp.Cameras)
                                    .ToListAsync();
                ViewBag.Checkpoints = checkpoints;

                var checklists = await _context.CheckLists.Include(c => c.ModelNavigation).ToListAsync();
                ViewBag.Checklists = checklists;

                var cameras = await _context.Cameras.ToListAsync();
                ViewBag.Cameras = cameras;

                var locations = await _context.Locations.ToListAsync();
                ViewBag.Locations = locations;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCheckpoint(string CheckpointName, List<int> txtCamera, List<int> txtChecklist, int txtlevel)
        {
            try
            {

                if (txtlevel <= 0)
                {
                    return BadRequest("Invalid level selected.");
                }
                var newCheckpoint = new CheckPoint
                {
                    CheckPointName = CheckpointName,
                    LocationId = txtlevel
                };

                _context.CheckPoints.Add(newCheckpoint);
                await _context.SaveChangesAsync();

                // Load existing cameras and checklists from the database
                var cameras = await _context.Cameras.Where(c => txtCamera.Contains(c.CameraId)).ToListAsync();
                var checklists = await _context.CheckLists.Where(cl => txtChecklist.Contains(cl.CheckListId)).ToListAsync();

                newCheckpoint.Cameras = cameras;
                newCheckpoint.CheckLists = checklists;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCheckPoint(int CheckpointId, bool forceDelete = false)
        {
            try
            {
                var checkpoint = await _context.CheckPoints
                    .Include(cp => cp.Cameras)
                    .Include(cp => cp.CheckLists)
                    .FirstOrDefaultAsync(cp => cp.CheckPointId == CheckpointId);

                if (checkpoint == null)
                {
                    return NotFound();
                }

                // Check if the checkpoint is associated with any routes in Route_CheckPoint
                var associatedRouteIds = await _context.RouteCheckPoints
                    .Where(rcp => rcp.CheckPointId == CheckpointId)
                    .Select(rcp => rcp.RouteId)
                    .ToListAsync();

                if (associatedRouteIds.Any() && !forceDelete)
                {
                    // Retrieve the route names for the associated route IDs
                    var routeNames = await _context.Routes
                        .Where(r => associatedRouteIds.Contains(r.RouteId))
                        .Select(r => r.RouteName)
                        .ToListAsync();

                    // Return JSON response with route information
                    return Json(new
                    {
                        success = false,
                        message = "This checkpoint is currently used by the following route(s): " +
                                 string.Join(", ", routeNames) +
                                 ". Do you want to continue with deletion? This will remove the checkpoint from these routes.",
                        routeNames = routeNames
                    });
                }

                // If forceDelete is true or no associations exist, proceed with deletion
                if (associatedRouteIds.Any())
                {
                    // For forced deletion, remove RouteCheckPoints entries
                    var routeCheckPoints = await _context.RouteCheckPoints
                        .Where(rcp => rcp.CheckPointId == CheckpointId)
                        .ToListAsync();

                    _context.RouteCheckPoints.RemoveRange(routeCheckPoints);
                    await _context.SaveChangesAsync();
                }

                // Clear related entities and delete the checkpoint
                checkpoint.Cameras.Clear();
                checkpoint.CheckLists.Clear();

                _context.CheckPoints.Remove(checkpoint);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Checkpoint deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to delete the checkpoint. Please try again."
                });
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> EditCheckpoint(int CheckpointId, string CheckpointName, List<int> txtEditCamera, List<int> txtEditChecklist, int txtEditLevel)
        //{
        //    try
        //    {
        //        // Find the existing checkpoint
        //        var existingCheckpoint = await _context.CheckPoints
        //            .Include(cp => cp.Cameras)
        //            .Include(cp => cp.CheckLists)
        //            .FirstOrDefaultAsync(cp => cp.CheckPointId == CheckpointId);

        //        if (existingCheckpoint == null)
        //        {
        //            return NotFound();
        //        }

        //        existingCheckpoint.CheckPointName = CheckpointName;
        //        existingCheckpoint.LocationId = txtEditLevel;

        //        existingCheckpoint.Cameras.Clear();
        //        existingCheckpoint.CheckLists.Clear();

        //        var cameras = await _context.Cameras.Where(c => txtEditCamera.Contains(c.CameraId)).ToListAsync();
        //        var checklists = await _context.CheckLists.Where(cl => txtEditChecklist.Contains(cl.CheckListId)).ToListAsync();

        //        foreach (var camera in cameras)
        //        {
        //            existingCheckpoint.Cameras.Add(camera);
        //        }

        //        foreach (var checklist in checklists)
        //        {
        //            existingCheckpoint.CheckLists.Add(checklist);
        //        }

        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Error: {ex.Message}");
        //        Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
        //        if (ex is DbUpdateException dbEx)
        //        {
        //            return StatusCode(500, new { message = "Database update failed: " + dbEx.InnerException?.Message });
        //        }
        //        return StatusCode(500, new { message = "An error occurred while updating the checkpoint: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> EditCheckpoint(int CheckpointId, string CheckpointName, List<int> txtEditCamera, List<int> txtEditChecklist, int txtEditLevel)
        {
            try
            {
                // Find the existing checkpoint
                var checkpoint = await _context.CheckPoints
                    .Include(c => c.Cameras)  // Include related cameras
                    .Include(c => c.CheckLists)  // Include related checklists
                    .FirstOrDefaultAsync(c => c.CheckPointId == CheckpointId);

                if (checkpoint == null)
                {
                    return NotFound("Checkpoint not found");
                }

                // Update checkpoint basic information
                checkpoint.CheckPointName = CheckpointName;
                checkpoint.LocationId = txtEditLevel;  // Assuming txtEditLevel is the LocationId

                // Update associated cameras
                checkpoint.Cameras.Clear();

                // Then, add new cameras based on the IDs provided in txtEditCamera
                if (txtEditCamera != null && txtEditCamera.Any())
                {
                    var camerasToAdd = await _context.Cameras
                        .Where(c => txtEditCamera.Contains(c.CameraId))
                        .ToListAsync();
                    foreach (var camera in camerasToAdd)
                    {
                        checkpoint.Cameras.Add(camera);
                    }
                }

                // Update associated checklists
                checkpoint.CheckLists.Clear(); // Clear existing checklists
                if (txtEditChecklist != null && txtEditChecklist.Any())
                {
                    var checklistsToAdd = await _context.CheckLists
                        .Where(cl => txtEditChecklist.Contains(cl.CheckListId)) // Assuming CheckListId is the primary key
                        .ToListAsync();
                    foreach (var checklist in checklistsToAdd)
                    {
                        checkpoint.CheckLists.Add(checklist);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Checkpoints");  // Or return appropriate response
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app, use proper logging)
                return BadRequest($"Error updating checkpoint: {ex.Message}");
            }
        }

        public IActionResult GetLocationLists(int locationId)
        {
            try
            {
                var location = _context.Locations
                    .FirstOrDefault(l => l.LocationId == locationId);

                var blocks = _context.Locations
                    .Where(l => l.LocationId == locationId)
                    .Select(l => new
                    {
                        LocationId = l.LocationId,
                        Name = l.Name,
                        Level = l.Level
                    })
                    .ToList();

                var allLocations = _context.Locations
                    .Select(l => new
                    {
                        LocationId = l.LocationId,
                        Name = l.Name,
                        Level = l.Level
                    })
                    .ToList();

                var response = new
                {
                    RelatedBlocks = blocks,
                    AllBlocks = allLocations
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public IActionResult GetCameraLists(int checkPointId)
        {
            try
            {
                var checkPoint = _context.CheckPoints
                                         .Include(cp => cp.Cameras)
                                         .FirstOrDefault(cp => cp.CheckPointId == checkPointId);

                if (checkPoint == null)
                {
                    return NotFound();
                }

                var response = new
                {
                    RelatedCameras = checkPoint.Cameras.Select(c => new
                    {
                        CameraId = c.CameraId,
                        Name = c.Name
                    }).ToList(),
                    AllCameras = _context.Cameras.Select(c => new
                    {
                        CameraId = c.CameraId,
                        Name = c.Name
                    }).ToList()
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public IActionResult GetCheckLists(int checkPointId)
        {
            try
            {
                // Fetch the CheckPoint along with related CheckLists
                var checkPoint = _context.CheckPoints
                                         .Include(cp => cp.CheckLists)
                                         .FirstOrDefault(cp => cp.CheckPointId == checkPointId);

                if (checkPoint == null)
                {
                    return NotFound();
                }

                var response = new
                {
                    RelatedCheckLists = checkPoint.CheckLists.Select(cl => new
                    {
                        CheckListId = cl.CheckListId,
                        CheckListName = cl.CheckListName,
                        Type = cl.Type
                    }).ToList(),
                    AllCheckLists = _context.CheckLists.Select(cl => new
                    {
                        CheckListId = cl.CheckListId,
                        CheckListName = cl.CheckListName,
                        Type = cl.Type
                    }).ToList() // Ensure you have this to populate AllCheckLists
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // Add this new method to CheckpointsController
        public IActionResult GetCamerasByLocation(string block, string level)
        {
            try
            {
                var location = _context.Locations
                    .FirstOrDefault(l => l.Name == block && l.Level == level);

                if (location == null)
                {
                    return NotFound();
                }

                var cameras = _context.Cameras
                    .Where(c => c.LocationId == location.LocationId)
                    .Select(c => new
                    {
                        CameraId = c.CameraId,
                        Name = c.Name
                    })
                    .ToList();

                return Json(new { cameras = cameras });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}