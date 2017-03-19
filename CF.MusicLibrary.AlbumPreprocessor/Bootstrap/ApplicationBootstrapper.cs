using System;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.AlbumPreprocessor.Bootstrap
{
	internal class ApplicationBootstrapper : IApplicationBootstrapper
	{
		/// <summary>
		/// Property Injection for IUnityContainer.
		/// </summary>
		public IUnityContainer DiContainer { get; set; } = new UnityContainer();

		public void Run()
		{
			DiContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DiContainer.RegisterType<IEthalonSongParser, EthalonSongParser>();
			DiContainer.RegisterType<IEthalonAlbumParser, EthalonAlbumParser>();
			DiContainer.RegisterType<IAlbumContentParser, AlbumContentParser>();
			DiContainer.RegisterType<IInputContentSplitter, InputContentSplitter>();
			DiContainer.RegisterType<IAlbumContentComparer, AlbumContentComparer>();
			DiContainer.RegisterType<MainWindowModel>();
		}

		public TRootViewModel GetRootViewModel<TRootViewModel>()
		{
			return DiContainer.Resolve<TRootViewModel>();
		}

		/// <summary>
		/// Implementation for IDisposable.Dispose().
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases object resources.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False positive. See http://stackoverflow.com/q/34583417/5740031 for details.")]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DiContainer.Dispose();
			}
		}
	}
}
