using AdminPortalV8.Models.Epatrol;
using System.Diagnostics;

namespace AdminPortalV8.Services
{
    public interface IFfmpegProcessService
    {
        void StartProcess(int checkpointId, string cameraUrl);
        string ExtractFrame(string cameraUrl, string cameraId);
        void StopProcess();
        void StopProcess(int checkpointId);
    }

    public class FfmpegProcessService : IFfmpegProcessService
    {
        private readonly Dictionary<int, Process> _ffmpegProcesses = new Dictionary<int, Process>();

        public void StartProcess(int checkpointId, string cameraUrl)
        {
            //var testVideoUrl = @"C:\Users\imori\10.mp4";
            // Define paths for .ts and .m3u8 files
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "stream");
            var segmentPath = Path.Combine(outputDir, $"output_camera{checkpointId}_%03d.ts");
            //var segmentPath = Path.Combine(outputDir, $"output_camera{checkpointId}_%03d.ts").Replace("\\", "/");
            var playlistPath = Path.Combine(outputDir, $"output_camera{checkpointId}.m3u8");

            // Ensure the output directory exists
            Directory.CreateDirectory(outputDir);

            // Build the ffmpeg command
            var ffmpegArgs = $"-i \"{cameraUrl}\" -c:v copy -c:a aac -f hls -hls_time 2 -hls_list_size 5 " +
                                  $"-hls_flags delete_segments -hls_segment_filename \"{segmentPath}\" \"{playlistPath}\" -v verbose";

             // Start the ffmpeg process
             var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",  // Assumes ffmpeg is installed and available in PATH
                    Arguments = ffmpegArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.EnableRaisingEvents = true;
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine(e.Data); // Log output
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine(e.Data); // Log error output
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Store the process in the dictionary
            _ffmpegProcesses[checkpointId] = process;
        }

        public void StopProcess()
        {
            // Stop all existing FFmpeg processes
            foreach (var processEntry in _ffmpegProcesses.ToList()) // Use ToList to avoid modifying the collection while iterating
            {
                var checkpointId = processEntry.Key;
                var process = processEntry.Value;

                if (process != null && !process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit();
                }

                // Clean up the dictionary
                _ffmpegProcesses.Remove(checkpointId);
                Debug.WriteLine($"FFmpeg process stopped and removed for checkpointId: {checkpointId}");
            }

            // Clear the stream folder
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "stream");

            try
            {
                // Check if the directory exists
                if (Directory.Exists(outputDir))
                {
                    // Delete all files in the directory
                    foreach (var file in Directory.GetFiles(outputDir))
                    {
                        File.Delete(file);
                    }

                    // Optionally, delete subdirectories if needed
                    foreach (var directory in Directory.GetDirectories(outputDir))
                    {
                        Directory.Delete(directory, true); // true to delete recursively
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                Debug.WriteLine($"Error clearing stream folder: {ex.Message}");
            }
        }

        public void StopProcess(int checkpointId)
        {
            if (_ffmpegProcesses.TryGetValue(checkpointId, out var process))
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit();
                }

                _ffmpegProcesses.Remove(checkpointId);
                Debug.WriteLine($"FFmpeg process stopped and removed for checkpointId: {checkpointId}");

                // 删除对应文件
                var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "stream");
                var playlistFile = Path.Combine(outputDir, $"output_camera{checkpointId}.m3u8");

                if (File.Exists(playlistFile))
                {
                    File.Delete(playlistFile);
                }

                var segmentFiles = Directory.GetFiles(outputDir, $"output_camera{checkpointId}_*.ts");
                foreach (var file in segmentFiles)
                {
                    File.Delete(file);
                }
            }
        }


        public string ExtractFrame(string cameraUrl, string cameraId)
        {
            // Define the base output directory for frames
            var baseOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

            // Create a unique folder name using checkpointId and current timestamp
            var uniqueFolderName = $"camId{cameraId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            var outputDir = Path.Combine(baseOutputDir, uniqueFolderName);

            // Ensure the output directory exists
            Directory.CreateDirectory(outputDir);

            // Define the output file pattern for images
            var outputPattern = Path.Combine(outputDir, $"frame_%03d.jpg"); // e.g., frame_001.jpg, frame_002.jpg, etc.

            // Build the ffmpeg command
            var ffmpegArgs = $"-rtsp_transport tcp -i \"{cameraUrl}\" -frames:v 5 -q:v 2 \"{outputPattern}\"";

            // Start the ffmpeg process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",  // Assumes ffmpeg is installed and available in PATH
                    Arguments = ffmpegArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.EnableRaisingEvents = true;
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine(e.Data); // Log output
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine(e.Data); // Log error output
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait up to 10 seconds (10,000 ms) for FFmpeg to exit
            const int timeoutMs = 3_000;
            bool exited = process.WaitForExit(timeoutMs);

            if (!exited)
            {
                // If FFmpeg is still running after 10s, kill it
                try
                {
                    process.Kill();
                    Console.WriteLine("FFmpeg process killed due to timeout.");
                }
                catch
                {
                    // ignore kill exceptions
                }

                // Cleanup the output directory in case partial files are created
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                } 

                return null; // Indicate failure or throw an exception
            }

            if (process.ExitCode == 0)
            {
                return outputDir; // Return the directory path upon success
            }
            else
            {
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                }
                return null; // Indicate failure
            }
        }
    }
}
