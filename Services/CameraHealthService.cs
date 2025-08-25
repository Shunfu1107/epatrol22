using System;
using System.Collections.Generic;
using System.Linq;
using AdminPortalV8.Models.Epatrol;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Services
{
    public class CameraHealthService
    {
        private readonly EPatrol_DevContext _context;

        public CameraHealthService(EPatrol_DevContext context)
        {
            _context = context;
        }

        public void AddResult(string cameraId, string cameraName, string resultMessage)
        {
            var jobResult = new CameraHealthResult
            {
                CameraId = int.Parse(cameraId),
                CameraName = cameraName,
                Result = resultMessage,
                Timestamp = DateTime.Now
            };

            _context.CameraHealthResults.Add(jobResult);
            _context.SaveChanges();
        }

    }

}
