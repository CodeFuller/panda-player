using System.Collections.Generic;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ICreateAdviseGroupViewModel
	{
		public string AdviseGroupName { get; set; }

		public IReadOnlyCollection<string> ExistingAdviseGroupNames { get; }

		void Load(string initialAdviseGroupName, IEnumerable<string> existingAdviseGroupNames);
	}
}
