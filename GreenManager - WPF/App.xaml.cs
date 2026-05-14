using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace GreenManager___WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var culture = new CultureInfo("nl-BE");

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			FrameworkElement.LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
		}
	}

}
