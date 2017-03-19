using CF.Library.Core.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor.Bootstrap
{
	internal interface IApplicationBootstrapper : IBootstrapper
	{
		TRootViewModel GetRootViewModel<TRootViewModel>();
	}
}
