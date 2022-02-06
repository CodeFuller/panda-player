using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal sealed class RawReferenceContentViewModel : ViewModelBase, IRawReferenceContentViewModel, IDisposable
	{
		private const string RawReferenceContentDataKey = "RawReferenceContentData";

		private readonly ISessionDataService sessionDataService;

		private readonly System.Timers.Timer saveContentTimer;

		public RawReferenceContentViewModel(ISessionDataService sessionDataService)
		{
			this.sessionDataService = sessionDataService ?? throw new ArgumentNullException(nameof(sessionDataService));

			saveContentTimer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds)
			{
				AutoReset = true,
				Enabled = true,
			};

			saveContentTimer.Elapsed += (s, e) => OnSaveContentTimerElapsed(CancellationToken.None);
		}

		private string rawReferenceContent;

		public string Content
		{
			get => rawReferenceContent;
			set => Set(ref rawReferenceContent, value);
		}

		private string LastSavedContent { get; set; }

		public async Task LoadRawReferenceContent(CancellationToken cancellationToken)
		{
			Content = LastSavedContent = await sessionDataService.GetData<string>(RawReferenceContentDataKey, cancellationToken);
		}

		private async void OnSaveContentTimerElapsed(CancellationToken cancellationToken)
		{
			if (String.Equals(Content, LastSavedContent, StringComparison.Ordinal))
			{
				return;
			}

			LastSavedContent = Content;
			await sessionDataService.SaveData(RawReferenceContentDataKey, LastSavedContent, cancellationToken);
		}

		public void Dispose()
		{
			saveContentTimer?.Dispose();
		}
	}
}
