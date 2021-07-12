using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class EditDiscPropertiesViewModel : ViewModelBase, IEditDiscPropertiesViewModel
	{
		private readonly IDiscsService discsService;

		private DiscModel Disc { get; set; }

		private string title;

		public string Title
		{
			get => title;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of disc title could not be empty");
				}

				Set(ref title, value);
			}
		}

		private string treeTitle;

		public string TreeTitle
		{
			get => treeTitle;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of disc tree title could not be empty");
				}

				Set(ref treeTitle, value);
			}
		}

		private string albumTitle;

		public string AlbumTitle
		{
			get => albumTitle;
			set
			{
				if (value?.Length == 0)
				{
					value = null;
				}

				Set(ref albumTitle, value);
			}
		}

		private int? year;

		public int? Year
		{
			get => year;
			set => Set(ref year, value);
		}

		public EditDiscPropertiesViewModel(IDiscsService discsService)
		{
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
		}

		public void Load(DiscModel disc)
		{
			Disc = disc;
			Title = disc.Title;
			TreeTitle = disc.TreeTitle;
			AlbumTitle = disc.AlbumTitle;
			Year = disc.Year;
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			Disc.TreeTitle = TreeTitle;
			Disc.Title = Title;
			Disc.AlbumTitle = AlbumTitle;
			Disc.Year = Year;

			await discsService.UpdateDisc(Disc, cancellationToken);
		}
	}
}
