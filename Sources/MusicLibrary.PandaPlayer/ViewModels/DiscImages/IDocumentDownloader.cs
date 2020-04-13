using System;
using System.Threading.Tasks;

namespace MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	public interface IDocumentDownloader
	{
		Task<byte[]> Download(Uri documentUri);
	}
}
