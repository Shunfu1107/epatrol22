using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Controllers
{
    public class LocationController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        public LocationController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
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
        public async Task<IActionResult> CreateNewLocation(string BlockName, List<string> Levels, List<IFormFile> patrolMap)
        {
            try
            {
                // Check if Levels and patrolMap have matching counts
                if (Levels == null || Levels.Count == 0)
                {
                    return BadRequest("Levels cannot be null or empty.");
                }

                if (patrolMap == null || patrolMap.Count != Levels.Count)
                {
                    return BadRequest($"Mismatch: Levels.Count = {Levels.Count}, patrolMap.Count = {patrolMap?.Count}");
                }

                var existingLocation = await _context.Locations.FirstOrDefaultAsync(l => l.Name == BlockName);

                if (existingLocation != null)
                {
                    TempData["ErrorMessage"] = $"Block Name {BlockName} already exists.";
                    return RedirectToAction("Index");
                }

                for (int i = 0; i < Levels.Count; i++)
                {
                    byte[]? imageData = null;

                    if (patrolMap[i] != null && (patrolMap[i].ContentType == "image/jpeg" || patrolMap[i].ContentType == "image/png"))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await patrolMap[i].CopyToAsync(memoryStream);
                            imageData = memoryStream.ToArray();
                        }
                    }
                    else if (patrolMap[i] != null)
                    {
                        return BadRequest($"File at index {i} is not a valid image file. Only JPEG and PNG are allowed.");
                    }

                    var newLocation = new Models.Epatrol.Location
                    {
                        Name = BlockName,
                        Level = Levels[i],
                        Image = imageData
                    };

                    _context.Locations.Add(newLocation);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocation(string LocationIds)
        {
            try
            {
                var locationIds = LocationIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();

                foreach (var locationId in locationIds)
                {
                    var location = await _context.Locations
                        .FirstOrDefaultAsync(l => l.LocationId == locationId);

                    if (location != null)
                    {
                        _context.Locations.Remove(location);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Locations deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete the location. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditLocation(List<int> LocationIds, string BlockName, List<string> editLevels, IFormCollection form)
        {
            try
            {
                if (editLevels == null || editLevels.Count == 0)
                {
                    return BadRequest("Invalid data: Missing required fields.");
                }

                var newLocations = new List<Location>();

                for (int i = 0; i < editLevels.Count; i++)
                {
                    byte[]? imageData = null;

                    // Retrieve the uploaded file using the modified name (editPatrolMap-{i})
                    var patrolMapFile = form.Files.FirstOrDefault(f => f.Name == $"editPatrolMap-{i}");

                    if (patrolMapFile != null)
                    {
                        if (patrolMapFile.ContentType == "image/jpeg" || patrolMapFile.ContentType == "image/png")
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await patrolMapFile.CopyToAsync(memoryStream);
                                imageData = memoryStream.ToArray();
                            }
                        }
                        else
                        {
                            return BadRequest($"File at index {i} is not a valid image file. Only JPEG and PNG are allowed.");
                        }
                    }
                    else
                    {
                        // If no new image is uploaded, use the existing image (if available)
                        var existingImageValue = form[$"existingImage-{i}"].ToString();
                        if (!string.IsNullOrEmpty(existingImageValue))
                        {
                            try
                            {
                                // If the string has a "data:image/png;base64," prefix, strip it
                                var base64 = existingImageValue;
                                var prefix = "base64,";
                                var index = existingImageValue.IndexOf(prefix);
                                if (index >= 0) base64 = existingImageValue.Substring(index + prefix.Length);

                                base64 = base64.Trim();

                                imageData = Convert.FromBase64String(base64);
                            }
                            catch (FormatException)
                            {
                                return BadRequest($"Invalid base64 string at index {i}. Please re-upload the image.");
                            }
                        }
                    }

                    if (LocationIds != null && LocationIds.Count > i && LocationIds[i] != 0)
                    {
                        var existingLocation = await _context.Locations
                            .FirstOrDefaultAsync(l => l.LocationId == LocationIds[i]);

                        if (existingLocation != null)
                        {
                            existingLocation.Name = BlockName;
                            existingLocation.Level = editLevels[i];
                            if (imageData != null)
                            {
                                existingLocation.Image = imageData;
                            }

                            _context.Locations.Update(existingLocation);
                        }
                        else
                        {
                            return NotFound($"Location with ID {LocationIds[i]} not found.");
                        }
                    }
                    else
                    {
                        var newPatrolMapFile = form.Files.FirstOrDefault(f => f.Name == $"editPatrolMap");

                        if (newPatrolMapFile != null)
                        {
                            if (newPatrolMapFile.ContentType == "image/jpeg" || newPatrolMapFile.ContentType == "image/png")
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await newPatrolMapFile.CopyToAsync(memoryStream);
                                    imageData = memoryStream.ToArray();
                                }
                            }
                            else
                            {
                                return BadRequest($"File at index {i} is not a valid image file. Only JPEG and PNG are allowed.");
                            }
                        }
                        var newLocation = new Location
                        {
                            Name = BlockName,
                            Level = editLevels[i],
                            Image = imageData
                        };

                        newLocations.Add(newLocation);
                    }
                }

                var locationsWithSameName = await _context.Locations
                    .Where(l => l.Name == BlockName)
                    .ToListAsync();

                foreach (var location in locationsWithSameName)
                {
                    if (!LocationIds.Contains(location.LocationId))
                    {
                        _context.Locations.Remove(location);
                    }
                }

                if (newLocations.Any())
                {
                    _context.Locations.AddRange(newLocations);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
