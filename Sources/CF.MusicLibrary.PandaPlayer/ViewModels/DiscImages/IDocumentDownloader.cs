using System;
using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	public interface IDocumentDownloader
	{
		Task<byte[]> Download(Uri documentUri);
	}
}
