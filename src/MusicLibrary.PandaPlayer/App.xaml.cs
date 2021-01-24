using System;
using CodeFuller.Library.Wpf;
using MusicLibrary.PandaPlayer.ViewModels;

namespace MusicLibrary.PandaPlayer
{
	internal partial class App : WpfApplication<ApplicationViewModel>
	{
		public App()
#pragma warning disable CA2000 // Dispose objects before losing scope
			: base(new ApplicationBootstrapper())
#pragma warning restore CA2000 // Dispose objects before losing scope
		{
		}

		protected override void Run(ApplicationViewModel rootViewModel)
		{
			_ = rootViewModel ?? throw new ArgumentNullException(nameof(rootViewModel));

			var appView = new Views.ApplicationView(rootViewModel);
			appView.Show();
		}
	}
}
