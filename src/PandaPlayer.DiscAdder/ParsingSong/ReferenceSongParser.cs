using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace PandaPlayer.DiscAdder.ParsingSong
{
	internal class ReferenceSongParser : IReferenceSongParser
	{
		internal static ReadOnlyCollection<SongTitlePattern> TitlePatterns { get; } = new ReadOnlyCollection<SongTitlePattern>(
			new SongTitlePattern[]
			{
				new SongTitlePattern
				{
					Description = "Wikipedia: track, title, payload, length",
					Source = @"https://en.wikipedia.org/wiki/The_Human_Contradiction",
					Pattern = @"^\d+\.\s+""(.+?)""\s+\((.+?)\)\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("2.	\"Your Body Is A Battleground\" (featuring Marco Hietala)	3:50", "Your Body Is A Battleground (feat. Marco Hietala)"),
					},
				},

				new SongTitlePattern
				{
					Description = "Wikipedia: track, title, payload (optional), authors, length (optional)",
					Source = @"https://en.wikipedia.org/wiki/We_Are_the_Others",
					Pattern = @"^\d+\.\s+""(.+?)""\s+(?:\((.+?)\)\s+)?.+?(\s+\d+:\d+)?$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	\"Mother Machine\"	Martijn Westerholt, Charlotte Wessels, Guus Eikens, Tripod	4:34", "Mother Machine"),
						new SongParsingTest("15.	\"Shattered\" (live)	Westerholt	4:20", "Shattered (Live)"),
						new SongParsingTest("17.	\"Come Closer\" (live)	Westerholt, Wessels", "Come Closer (Live)"),
					},
				},

				new SongTitlePattern
				{
					Description = "Wikipedia: track, \"title\", length",
					Source = @"https://en.wikipedia.org/wiki/The_Human_Contradiction",
					Pattern = @"^\d+\.\s+""(.+?)""\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	\"Here Come The Vultures\"	6:05", "Here Come The Vultures"),
					},
				},

				new SongTitlePattern
				{
					Description = "Wikipedia: \"title\" – length",
					Source = @"https://en.wikipedia.org/wiki/Beast_Within",
					Pattern = @"^""(.+?)""\s+–\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("\"Forgotten Bride\" – 4:22", "Forgotten Bride"),
					},
				},

				new SongTitlePattern
				{
					Description = "Wikipedia: title - length",
					Source = @"https://en.wikipedia.org/wiki/Out_of_the_Ashes_(Katra_album)",
					Pattern = @"^(.+?)\s+-\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("One Wish Away - 4:09", "One Wish Away"),
					},
				},

				new SongTitlePattern
				{
					Description = "ru.wikipedia: track, «title», authors, length",
					Source = @"https://ru.wikipedia.org/wiki/Через_все_времена",
					Pattern = @"^\d+\.\s+«(.+?)»\s+.+\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	«Через все времена»	Маргарита Пушкина	Виталий Дубинин	5:42", "Через все времена"),
					},
				},

				new SongTitlePattern
				{
					Description = "ru.wikipedia: track, «title», length",
					Source = @"https://ru.wikipedia.org/wiki/Штормовое_предупреждение_(альбом)",
					Pattern = @"^\d+\.\s+«(.+?)»\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	«Интро»	0:16", "Интро"),
					},
				},

				new SongTitlePattern
				{
					Description = "Discogs: track, title, length",
					Source = @"https://www.discogs.com/Elysion-Someplace-Better/release/5489046",
					Pattern = @"^\d+\s+(.+?)\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1	Made Of Lies	3:19", "Made Of Lies"),
					},
				},

				new SongTitlePattern
				{
					Description = "Discogs: track, title",
					Source = @"https://www.discogs.com/Macbeth-Neo-Gothic-Propaganda/release/5671347",
					Pattern = @"^\d+\s+(.*?)\s*$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1	Scent Of Winter	", "Scent Of Winter"),
						new SongParsingTest("1	Scent Of Winter", "Scent Of Winter"),
					},
				},

				new SongTitlePattern
				{
					Description = "metal-archives.com: track, title, length, 'Show lyrics' link",
					Pattern = @"^\d+\.\s+(.+?)\s+\d+:\d+\s+Show lyrics$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	Schnee & Rosen	03:53	  Show lyrics", "Schnee & Rosen"),
					},
				},

				new SongTitlePattern
				{
					Description = "Track. Title",
					Source = @"http://xzona.su/alternative/36644-tracktor-bowling-2016-2016.html",
					Pattern = @"^\d+\.\s+(.+?)\s*$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("01. Напролом", "Напролом"),
					},
				},

				new SongTitlePattern
				{
					Description = "Quoted Title followed by optional data",
					Source = @"http://www.tracktorbowling.ru/discography/",
					Pattern = @"^""(.+?)""(?: .+)?$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("\"Смерти нет\" текст", "Смерти нет"),
					},
				},

				new SongTitlePattern
				{
					Description = "Title followed by tab and payload data",
					Pattern = @"^(.+?)\t+(.*)$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("Along Comes Mary	live", "Along Comes Mary (Live)"),
					},
				},

				new SongTitlePattern
				{
					Description = "Trimmed raw title",
					Pattern = @"^\s*(.+?)\s*$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("Mother Machine", "Mother Machine"),
					},
				},
			});

		public string ParseSongTitle(string rawSongTitle)
		{
			foreach (var pattern in TitlePatterns)
			{
				var match = pattern.Match(rawSongTitle, ParseSongPayload);
				if (match.Success)
				{
					return match.SongTitle;
				}
			}

			// We shouldn't get here, any possible title should be covered by TitlePatterns
			throw new InvalidOperationException($"Title {rawSongTitle} is not covered by any pattern");
		}

		private static string ParseSongPayload(string rawSongPayload)
		{
			if (rawSongPayload == "live")
			{
				return "(Live)";
			}

			var match = new Regex("^live, featuring (.+?)$").Match(rawSongPayload);
			if (match.Success)
			{
				return $"(feat. {match.Groups[1].Value}) (Live)";
			}

			match = new Regex("^featuring (.+?)$").Match(rawSongPayload);
			if (match.Success)
			{
				return $"(feat. {match.Groups[1].Value})";
			}

			return $"({rawSongPayload})";
		}
	}
}
