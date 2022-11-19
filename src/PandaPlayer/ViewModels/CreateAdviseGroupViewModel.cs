using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class CreateAdviseGroupViewModel : ViewModelBase, ICreateAdviseGroupViewModel
	{
		private string adviseGroupName;

		public string AdviseGroupName
		{
			get => adviseGroupName;
			set => Set(ref adviseGroupName, value);
		}

		public IReadOnlyCollection<string> ExistingAdviseGroupNames { get; private set; }

		public void Load(string initialAdviseGroupName, IEnumerable<string> existingAdviseGroupNames)
		{
			ExistingAdviseGroupNames = existingAdviseGroupNames.ToList();
			AdviseGroupName = initialAdviseGroupName;
		}
	}
}
