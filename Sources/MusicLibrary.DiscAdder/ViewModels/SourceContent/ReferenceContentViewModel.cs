using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.DiscAdder.ViewModels.SourceContent
{
	internal sealed class ReferenceContentViewModel : ViewModelBase, IReferenceContentViewModel, IDisposable
	{
		private const string RawReferenceDiscsDataKey = "RawReferenceDiscsData";

		private readonly ISessionDataService sessionDataService;

		private readonly System.Timers.Timer saveContentTimer;

		public ReferenceContentViewModel(ISessionDataService sessionDataService)
		{
			this.sessionDataService = sessionDataService ?? throw new ArgumentNullException(nameof(sessionDataService));

			saveContentTimer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds)
			{
				AutoReset = true,
				Enabled = true,
			};

			saveContentTimer.Elapsed += (s, e) => OnSaveContentTimerElapsed(CancellationToken.None);
		}

		private string rawReferenceDiscsContent;

		public string Content
		{
			get => rawReferenceDiscsContent;
			set => Set(ref rawReferenceDiscsContent, value);
		}

		private string LastSavedContent { get; set; }

		public async Task LoadRawReferenceDiscsContent(CancellationToken cancellationToken)
		{
			Content = LastSavedContent = await sessionDataService.GetData<string>(RawReferenceDiscsDataKey, cancellationToken);
		}

		private async void OnSaveContentTimerElapsed(CancellationToken cancellationToken)
		{
			if (String.Equals(Content, LastSavedContent, StringComparison.Ordinal))
			{
				return;
			}

			LastSavedContent = Content;
			await sessionDataService.SaveData(RawReferenceDiscsDataKey, LastSavedContent, cancellationToken);
		}

		public void Dispose()
		{
			saveContentTimer?.Dispose();
		}
	}
}
