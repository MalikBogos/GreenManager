namespace GreenManager_Web.Models
{
	public class ErrorViewModel // default inbegrepen
	{
		public string? RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}
