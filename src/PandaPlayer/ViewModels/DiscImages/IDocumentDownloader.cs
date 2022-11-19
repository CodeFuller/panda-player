using System;
using System.Threading.Tasks;

namespace PandaPlayer.ViewModels.DiscImages
{
	public interface IDocumentDownloader
	{
		Task<byte[]> Download(Uri documentUri);
	}
}
