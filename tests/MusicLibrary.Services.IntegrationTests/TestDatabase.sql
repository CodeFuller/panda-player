-- Create database tables with src\MusicLibrary.Dal.LocalDb\MusicLibrary.sql

INSERT INTO [Folders]([Id], [ParentFolder_Id], [Name], [DeleteDate]) VALUES
(2, 1, 'Foreign', NULL),
(3, 2, 'Guano Apes', NULL),
(4, 3, 'Empty Folder', NULL),
(5, 3, 'Deleted Folder', '2021-06-30 18:08:10');

INSERT INTO [Discs]([Id], [Folder_Id], [Year], [Title], [TreeTitle], [AlbumTitle]) VALUES
(1, 3, 2004, 'Planet Of The Apes - Best Of Guano Apes (CD 1)', '2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)', 'Planet Of The Apes - Best Of Guano Apes'),
(2, 3, NULL, 'Disc With Null Values', 'Disc With Null Values (CD 1)', NULL),
(3, 3, 2021, 'Deleted Disc', '2021 - Deleted Disc', 'Deleted Disc');

INSERT INTO [Artists] ([Id], [Name]) VALUES
(1, 'Guano Apes'),
(2, 'Neuro Dubel');

INSERT INTO [Genres] ([Id], [Name]) VALUES
(1, 'Alternative Rock'),
(2, 'Rock');

INSERT INTO [Songs] ([Id], [Artist_Id], [Disc_Id], [Genre_Id], [TrackNumber], [Title], [TreeTitle], [Duration],
			[Rating], [FileSize], [Checksum], [Bitrate], [LastPlaybackTime], [PlaybacksCount], [DeleteDate]) VALUES
(1, 1, 1, 1, 1, 'Break The Line', '01 - Break The Line.mp3', 211957, 6, 8479581, 292181681, 320000, '2021-04-03 10:33:53.3517221+03:00', 2, NULL),
(2, NULL, 1, NULL, NULL, 'Song With Null Values', 'Song With Null Values.mp3', 186697, NULL, 7469164, -1400931728, 320000, NULL, 0, NULL);

INSERT INTO [Songs] ([Id], [Artist_Id], [Disc_Id], [Genre_Id], [TrackNumber], [Title], [TreeTitle], [Duration],
			[Rating], [FileSize], [Checksum], [Bitrate], [LastPlaybackTime], [PlaybacksCount], [DeleteDate]) VALUES
(3, NULL, 2, NULL, NULL, 'Song From Null Disc', 'Song From Null Disc.mp3', 186697, NULL, 7469164, -1400931728, 320000, NULL, 0, NULL);

INSERT INTO [Songs] ([Id], [Artist_Id], [Disc_Id], [Genre_Id], [TrackNumber], [Title], [TreeTitle], [Duration],
			[Rating], [FileSize], [Checksum], [Bitrate], [LastPlaybackTime], [PlaybacksCount], [DeleteDate]) VALUES
(4, 2, 3, 2, 1, 'Deleted Song', '01 - Deleted Song.mp3', 486739, 4, NULL, NULL, NULL, '2021-03-28 09:33:39.2582742+03:00', 1, '2021-03-28 14:10:59.3191807+03:00');

INSERT INTO [DiscImages] ([Id], [Disc_Id], [TreeTitle], [ImageType], [FileSize], [Checksum]) VALUES
(1, 1, 'cover.jpg', 1, 15843, -2061102933);

INSERT INTO [Playbacks] ([Id], [Song_Id], [PlaybackTime]) VALUES
(1, 1, '2021-03-19 13:35:02.2626013+03:00'),
(2, 1, '2021-04-03 10:33:53.3517221+03:00'),
(3, 3, '2021-03-28 09:33:39.2582742+03:00');
