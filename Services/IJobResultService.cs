using AdminPortalV8.Models;
using AdminPortalV8.Models.Epatrol;


public interface IJobResultService
{
    void AddResult(string cameraId, string cameraName, string status);
    List<JobResult> GetResults();
}

public class JobResultService : IJobResultService
{
    private readonly List<JobResult> _results = new List<JobResult>();

    private readonly EPatrol_DevContext _context;

    public JobResultService(EPatrol_DevContext context)
    {
        _context = context;
    }

    //public void AddResult(string cameraId, string cameraName, string status)
    //{
    //    var timestamp = DateTime.UtcNow;
    //    var location = _context.Cameras
    //        .Where(c => c.CameraId.ToString() == cameraId)
    //        .Select(c => c.Location != null ? c.Location.Name + " " + c.Location.Level : "Unknown")
    //        .FirstOrDefault();

    //    // Prevent duplicate entries
    //    bool exists = _context.Notifications.Any(n =>
    //        n.Device == cameraName &&
    //        n.Type == status &&
    //        n.Timestamp == timestamp &&
    //        n.Location == location
    //    );

    //    if (!exists)
    //    {
    //        _context.Notifications.Add(new Notification
    //        {
    //            CameraId = int.TryParse(cameraId, out int cid) ? cid : (int?)null,
    //            Device = cameraName,
    //            Type = status,
    //            Location = location,
    //            Timestamp = timestamp,
    //            CreatedAt = DateTime.Now
    //        });

    //        _context.SaveChanges();
    //    }

    //    _results.Add(new JobResult
    //    {
    //        CameraId = cameraId,
    //        CameraName = cameraName,
    //        Status = status,
    //        Timestamp = DateTime.UtcNow
    //    });


    //}

    public void AddResult(string cameraId, string cameraName, string status)
    {
        _results.Add(new JobResult
        {
            CameraId = cameraId,
            CameraName = cameraName,
            Status = status,
            Timestamp = DateTime.UtcNow
        });


    }

    public List<JobResult> GetResults()
    {
        return _results
            .GroupBy(r => r.CameraId)
            .Select(g => g.OrderByDescending(r => r.Timestamp).First())
            .ToList();
    }
}

public class JobResult
{
    public string CameraId { get; set; }
    public string CameraName { get; set; }
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
}