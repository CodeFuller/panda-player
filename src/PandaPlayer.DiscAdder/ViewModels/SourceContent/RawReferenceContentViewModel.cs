using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal sealed class RawReferenceContentViewModel : ObservableObject, IRawReferenceContentViewModel, IDisposable
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
			set => SetProperty(ref rawReferenceContent, value);
		}

		private string LastSavedContent { get; set; }

		public async Task LoadContent(CancellationToken cancellationToken)
		{
			Content = LastSavedContent = await sessionDataService.GetData<string>(RawReferenceContentDataKey, cancellationToken);
		}

		public Task ClearContent(CancellationToken cancellationToken)
		{
			return SaveContent(String.Empty, cancellationToken);
		}

		private async void OnSaveContentTimerElapsed(CancellationToken cancellationToken)
		{
			await SaveContent(Content, cancellationToken);
		}

		private async Task SaveContent(string newContent, CancellationToken cancellationToken)
		{
			if (LastSavedContent == newContent)
			{
				return;
			}

			await sessionDataService.SaveData(RawReferenceContentDataKey, newContent, cancellationToken);

			LastSavedContent = newContent;
		}

		public void Dispose()
		{
			saveContentTimer?.Dispose();
		}
	}
}
