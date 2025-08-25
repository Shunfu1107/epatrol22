using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Data;
using System;
using AdminPortalV8.Models.Epatrol;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace AdminPortalV8.Controllers
{
    public class FindpersonController : Controller
    {
        private readonly EPatrol_DevContext _context;

        public FindpersonController(EPatrol_DevContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetching Patrol data and passing it to the view
            var cameras = await _context.Cameras.ToListAsync();

            return View(cameras);
        }

        [HttpPost]
        public async Task<IActionResult> RunPythonDbScript(IFormFile uploadedImage)
        {
            var cameras = await _context.Cameras.ToListAsync();
            try
            {
                // Ensure the uploaded file is not null
                if (uploadedImage == null || uploadedImage.Length == 0)
                {
                    ViewBag.Error = "Please upload a valid image file.";
                    return View("~/Views/FindPerson/Index.cshtml");
                }

                // Determine the folder path for saving the image
                string faceImagesDbFolder = Path.Combine(Directory.GetCurrentDirectory(), "face_images_db");

                // Ensure the directory exists
                if (!Directory.Exists(faceImagesDbFolder))
                {
                    Directory.CreateDirectory(faceImagesDbFolder);
                }

                // Save the uploaded image in the 'face_images_db' folder
                string filePath = Path.Combine(faceImagesDbFolder, uploadedImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadedImage.CopyTo(stream);
                }

                // Dynamically determine the Python executable path
                string pythonExe = Path.Combine(Directory.GetCurrentDirectory(), "random_face_detection", ".venv", "Scripts", "python.exe");

                // Construct relative paths for the script and configuration files
                string scriptDir = Path.Combine(Directory.GetCurrentDirectory(), "random_face_detection", "face_recognition");
                string script = "index_faces.py";
                string configPath = Path.Combine(scriptDir, "config_local.yaml");

                // Use the 'face_images_db' folder as the database source
                string dbSource = faceImagesDbFolder;

                // Start the Python process
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{Path.Combine(scriptDir, script)}\" --config \"{configPath}\" --input_path \"{dbSource}\"",
                    WorkingDirectory = scriptDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    ViewBag.Output = output;
                    ViewBag.Errors = errors;
                }

                return View("~/Views/FindPerson/Index.cshtml", cameras);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Exception: {ex.Message}";
                return View("~/Views/FindPerson/Index.cshtml", cameras);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> StopPythonScript()
        //{
        //    try
        //    {
        //        // Get all processes with the name "python"
        //        var pythonProcesses = Process.GetProcessesByName("python"); // Use "python3" if applicable
        //        int terminatedCount = 0;

        //        foreach (var pythonProcess in pythonProcesses)
        //        {
        //            try
        //            {
        //                if (!pythonProcess.HasExited)
        //                {
        //                    pythonProcess.Kill();
        //                    pythonProcess.WaitForExit(); // Ensure the process is fully terminated
        //                    terminatedCount++;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log the error (optional)
        //                Console.WriteLine($"Error stopping process with ID {pythonProcess.Id}: {ex.Message}");
        //            }
        //        }

        //        if (terminatedCount > 0)
        //        {
        //            ViewBag.Message = $"{terminatedCount} Python process(es) have been terminated.";
        //        }
        //        else
        //        {
        //            ViewBag.Message = "No running Python processes were found.";
        //        }

        //        // Re-fetch cameras after stopping the Python script
        //        var cameras = await _context.Cameras.ToListAsync();
        //        ViewData["Cameras"] = cameras;
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = $"Error stopping processes: {ex.Message}";
        //    }

        //    return View("~/Views/FindPerson/Index.cshtml");
        //}

        [HttpPost]
        public async Task<IActionResult> StopPythonScript()
        {
            try
            {
                string projectDir = Directory.GetCurrentDirectory();
                // Create the stop flag file to signal Python script to finish
                string stopFlagPath = Path.Combine(projectDir, "random_face_detection", "face_recognition", "stop_flag.txt");
                System.IO.File.Create(stopFlagPath).Close();  // Create an empty file

                // Wait a little to allow the Python script to detect the stop signal
                await Task.Delay(6000);  // Adjust the delay as needed

                // Get all processes with the name "python"
                var pythonProcesses = Process.GetProcessesByName("python"); // Use "python3" if applicable
                int terminatedCount = 0;

                foreach (var pythonProcess in pythonProcesses)
                {
                    try
                    {
                        if (!pythonProcess.HasExited)
                        {
                            pythonProcess.Kill();
                            pythonProcess.WaitForExit(); // Ensure the process is fully terminated
                            terminatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error (optional)
                        Console.WriteLine($"Error stopping process with ID {pythonProcess.Id}: {ex.Message}");
                    }
                }

                if (System.IO.File.Exists(stopFlagPath))
                {
                    System.IO.File.Delete(stopFlagPath); // Delete the stop_flag.txt file
                    Console.WriteLine("stop_flag.txt has been deleted.");
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error stopping processes: {ex.Message}";
            }
            return RedirectToAction("Index", "Findperson");
        }

        [HttpPost]
        public async Task<IActionResult> RunPythonScript(List<string> Cameras)
        {
            var cameras = await _context.Cameras.ToListAsync();
            try
            {
                string projectDir = Directory.GetCurrentDirectory();
                string wwwrootPath = Path.Combine(projectDir, "wwwroot");
                string outputFolder = Path.Combine(wwwrootPath, "output");
                string personFoundFile = Path.Combine(projectDir, "random_face_detection", "face_recognition", "person_found.txt");

                // Ensure the output directory exists
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // Paths for Python execution
                string pythonExe = Path.Combine(projectDir, "random_face_detection", ".venv", "Scripts", "python.exe");
                string scriptDir = Path.Combine(projectDir, "random_face_detection", "face_recognition");
                string script = "identify_faces.py";
                string configPath = Path.Combine(scriptDir, "config_local.yaml");

                List<Task> processTasks = new List<Task>();

                foreach (var camera in Cameras)
                {
                    // Find the corresponding camera object by name
                    var cameraObject = cameras.FirstOrDefault(c => c.Url == camera);
                    if (cameraObject == null)
                    {
                        continue; // Skip if the camera is not found
                    }

                    // Create a file name using the camera name, ensuring it's file-system safe
                    string sanitizedCameraName = string.Concat(cameraObject.Name.Split(Path.GetInvalidFileNameChars()));
                    string outputFilePath = Path.Combine(outputFolder, $"output_{sanitizedCameraName}.mp4");

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = pythonExe,
                        Arguments = $"\"{Path.Combine(scriptDir, script)}\" --config \"{configPath}\" --video_source \"{camera}\" --save_output --output_video_path \"{outputFilePath}\"",
                        WorkingDirectory = scriptDir,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Run each process as a task
                    processTasks.Add(Task.Run(() =>
                    {
                        using (Process process = Process.Start(psi))
                        {
                            string output = process.StandardOutput.ReadToEnd();
                            string errors = process.StandardError.ReadToEnd();

                            process.WaitForExit();

                            // Log the results if needed
                            Console.WriteLine($"Output for {camera}:\n{output}");
                            Console.WriteLine($"Errors for {camera}:\n{errors}");
                        }
                    }));
                }

                // Monitor for person detection
                bool personFound = false;
                string personName = null;
                while (!personFound)
                {
                    if (System.IO.File.Exists(personFoundFile))
                    {
                        personFound = true;
                        personName = System.IO.File.ReadAllText(personFoundFile);
                        break;
                    }

                    await Task.Delay(5000); // Wait before checking again
                }

                await StopPythonScript();

                // Wait for all processes to complete
                Task.WaitAll(processTasks.ToArray());

                if (System.IO.File.Exists(personFoundFile))
                {
                    System.IO.File.Delete(personFoundFile); // Delete the person_found.txt file
                }

                ViewBag.Message = $"Person Found: {personName}";
                return View("~/Views/FindPerson/Index.cshtml", cameras);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred while running the Python scripts: {ex.Message}";
                return View("~/Views/FindPerson/Index.cshtml", cameras);
            }
        }
    }
}
