namespace ShiftScheduler.Models
{
    public class ErrorViewModel
    {
        // The RequestId is set by the framework when an error occurs.
        public string RequestId { get; set; } = string.Empty;

        // Show the RequestId in the UI only if it's not empty
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
