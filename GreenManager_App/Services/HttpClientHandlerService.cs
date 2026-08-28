namespace GreenManager_App.Services
{
	/// <summary>
	/// Wordt gebruikt om tijdens Android development API requests te sturen naar een lokale API (zelfondertekend SSL-certificaat wordt standaard geblokkeerd).
	/// </summary>
	public class HttpsClientHandlerService
	{
		/// <summary>
		/// Gebruikt HttpMessageHandler om certificaatvalidatie voor Android localhost te aanvaarden.
		/// </summary>
		/// <returns>Xamarin.Android.Net.AndroidMessageHandler met aangepaste certificaatvalidatie voor Android, anders standaard HttpClientHandler.</returns>
		public HttpMessageHandler GetPlatformMessageHandler()
		{
#if ANDROID
			var handler = new Xamarin.Android.Net.AndroidMessageHandler();
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
			{
				if (cert != null && cert.Issuer.Equals("CN=localhost"))
					return true;
				return errors == System.Net.Security.SslPolicyErrors.None;
			};
			return handler;
#else
            return new HttpClientHandler();
#endif
		}
	}
}