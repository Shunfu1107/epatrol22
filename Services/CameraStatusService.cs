//using System;
//using System.Collections.Generic;
//using System.Linq;
//using AdminPortalV8.Models.Epatrol;
//using Microsoft.EntityFrameworkCore;

//namespace AdminPortalV8.Services
//{
//    public class CameraStatusService
//    {
//        private readonly EPatrol_DevContext _context;

//        public CameraStatusService(EPatrol_DevContext context)
//        {
//            _context = context;
//        }
//        public void SaveCameraStatus(int cameraId, string note)
//        {
//            string status;

//            if (note.Contains("not connected", StringComparison.OrdinalIgnoreCase))
//            {
//                status = "Not Working";
//            }
//            else if (note.Contains("working correctly", StringComparison.OrdinalIgnoreCase))
//            {
//                status = "Working";
//            }
//            else if (note.Contains("view is blurry", StringComparison.OrdinalIgnoreCase))
//            {
//                status = "Blurry";
//            }
//            else if (note.Contains("view is dark", StringComparison.OrdinalIgnoreCase))
//            {
//                status = "Dark";
//            }
//            else
//            {
//                status = "Unknown"; // Optional fallback status
//            }

//            var cameraStatus = new CameraStatus
//            {
//                CameraId = cameraId,
//                Note = note,
//                Status = status,
//                StatusDate = DateTime.Now
//            };

//            _context.CameraStatuses.Add(cameraStatus);
//            _context.SaveChanges();
//        }


//        public void SaveNonWorkingCameras(List<string> failedCameraNames)
//{
//    var cameras = _context.Cameras
//        .Where(c => failedCameraNames.Contains(c.Name))
//        .Select(c => new { c.CameraId, c.Name })
//        .ToList();

//    foreach (var camera in cameras)
//    {
//        string note = $"{camera.Name} is not connected. Please check the power supply or cable connection.";
//        SaveCameraStatus(camera.CameraId, note);
//    }
//}

//    }
//}
