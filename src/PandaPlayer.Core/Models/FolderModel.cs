using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PandaPlayer.Core.Extensions;

namespace PandaPlayer.Core.Models
{
	public class FolderModel : BasicModel, INotifyPropertyChanged
	{
		private readonly List<FolderModel> subfolders = new();

		private readonly List<DiscModel> discs = new();

		public FolderModel ParentFolder { get; private set; }

		private string name;

		public string Name
		{
			get => name;
			set => this.SetField(PropertyChanged, ref name, value);
		}

		public IReadOnlyCollection<FolderModel> Subfolders
		{
			get => subfolders;
			private init => subfolders = new List<FolderModel>(value);
		}

		public IReadOnlyCollection<DiscModel> Discs
		{
			get => discs;
			private init => discs = new List<DiscModel>(value);
		}

		public bool HasContent => Subfolders.Any(f => !f.IsDeleted) || Discs.Any(d => !d.IsDeleted);

		private AdviseGroupModel adviseGroup;

		public AdviseGroupModel AdviseGroup
		{
			get => adviseGroup;
			set => this.SetField(PropertyChanged, ref adviseGroup, value);
		}

		public DateTimeOffset? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public bool IsRoot => ParentFolder == null;

		public IEnumerable<string> PathNames => PathNamesReversed.Reverse();

		private IEnumerable<string> PathNamesReversed
		{
			get
			{
				var currentFolder = this;
				while (!currentFolder.IsRoot)
				{
					yield return currentFolder.Name;
					currentFolder = currentFolder.ParentFolder;
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddSubfolder(FolderModel subfolder)
		{
			if (Subfolders.Any(x => x.Name == subfolder.Name))
			{
				throw new InvalidOperationException($"Cannot add subfolder with duplicated name: '{subfolder.Name}'");
			}

			subfolders.Add(subfolder);
			subfolder.ParentFolder = this;
		}

		public void AddDisc(DiscModel disc)
		{
			discs.Add(disc);
			disc.Folder = this;
		}

		public FolderModel CloneShallow()
		{
			return new()
			{
				Id = Id,
				ParentFolder = ParentFolder,
				Subfolders = Subfolders,
				Discs = Discs,
				Name = Name,
				AdviseGroup = AdviseGroup,
				DeleteDate = DeleteDate,
			};
		}
	}
}
