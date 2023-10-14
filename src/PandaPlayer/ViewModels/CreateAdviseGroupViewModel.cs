using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class CreateAdviseGroupViewModel : ObservableObject, ICreateAdviseGroupViewModel
	{
		private string adviseGroupName;

		public string AdviseGroupName
		{
			get => adviseGroupName;
			set => SetProperty(ref adviseGroupName, value);
		}

		public IReadOnlyCollection<string> ExistingAdviseGroupNames { get; private set; }

		public void Load(string initialAdviseGroupName, IEnumerable<string> existingAdviseGroupNames)
		{
			ExistingAdviseGroupNames = existingAdviseGroupNames.ToList();
			AdviseGroupName = initialAdviseGroupName;
		}
	}
}
