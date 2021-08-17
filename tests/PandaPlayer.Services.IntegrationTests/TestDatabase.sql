-- Read tests\PandaPlayer.Services.IntegrationTests\README.md

INSERT INTO [AdviseGroups] ([Id], [Name]) VALUES
(1, 'Folder Advise Group'),
(2, 'Disc Advise Group');

INSERT INTO [Folders] ([Id], [ParentFolder_Id], [AdviseGroup_Id], [Name], [DeleteDate]) VALUES
(2, 1, NULL, 'Belarusian', NULL),
(3, 2, 1, 'Neuro Dubel', NULL),
(4, 3, NULL, 'Empty Folder', NULL),
(5, 3, NULL, 'Deleted Folder', '2021-06-30 18:08:10');

INSERT INTO [Discs] ([Id], [Folder_Id], [AdviseGroup_Id], [Year], [Title], [TreeTitle], [AlbumTitle]) VALUES
(1, 3, 2, 2010, 'Афтары правды (CD 1)', '2010 - Афтары правды (CD 1)', 'Афтары правды'),
(2, 3, NULL, NULL, 'Disc With Missing Fields', 'Disc With Missing Fields (CD 1)', NULL),
(3, 3, NULL, 2021, 'Deleted Disc', '2021 - Deleted Disc', 'Deleted Disc');

INSERT INTO [Artists] ([Id], [Name]) VALUES
(1, 'Guano Apes'),
(2, 'Neuro Dubel');

INSERT INTO [Genres] ([Id], [Name]) VALUES
(1, 'Punk Rock'),
(2, 'Alternative Rock');

INSERT INTO [Songs] ([Id], [Artist_Id], [Disc_Id], [Genre_Id], [TrackNumber], [Title], [TreeTitle], [Duration],
			[Rating], [FileSize], [Checksum], [Bitrate], [LastPlaybackTime], [PlaybacksCount], [DeleteDate]) VALUES
(1, 2, 1, 1, 1, 'Про женщин', '01 - Про женщин.mp3', 10626, 6, 405582, 721007018, 320000, '2021-04-03 10:33:53.3517221+03:00', 2, NULL),
(2, 2, 1, 1, 2, 'Про жизнь дяди Саши', '02 - Про жизнь дяди Саши.mp3', 10600, 6, 404555, -465811692, 320000, '2021-04-03 10:37:42.1257252+03:00', 2, NULL);

INSERT INTO [Songs] ([Id], [Artist_Id], [Disc_Id], [Genre_Id], [TrackNumber], [Title], [TreeTitle], [Duration],
			[Rating], [FileSize], [Checksum], [Bitrate], [LastPlaybackTime], [PlaybacksCount], [DeleteDate]) VALUES
(3, NULL, 2, NULL, NULL, 'Song With Missing Fields', 'Song With Missing Fields.mp3', 11618, NULL, 445175, 751499818, 320000, NULL, 0, NULL);

INSERT INTO [Songs] ([Id], [Artist_Id], [Disc_Id], [Genre_Id], [TrackNumber], [Title], [TreeTitle], [Duration],
			[Rating], [FileSize], [Checksum], [Bitrate], [LastPlaybackTime], [PlaybacksCount], [DeleteDate]) VALUES
(4, 2, 3, 2, 1, 'Deleted Song', '01 - Deleted Song.mp3', 486739, 4, NULL, NULL, NULL, '2021-03-28 09:33:39.2582742+03:00', 1, '2021-03-28 14:10:59.3191807+03:00');

INSERT INTO [DiscImages] ([Id], [Disc_Id], [TreeTitle], [ImageType], [FileSize], [Checksum]) VALUES
(1, 1, 'cover.jpg', 1, 359119, -1502263015);

INSERT INTO [Playbacks] ([Id], [Song_Id], [PlaybackTime]) VALUES
(1, 1, '2021-03-19 13:35:02.2626013+03:00'),
(2, 2, '2021-03-19 13:39:13.1718232+03:00'),
(3, 4, '2021-03-28 09:33:39.2582742+03:00'),
(4, 1, '2021-04-03 10:33:53.3517221+03:00'),
(5, 2, '2021-04-03 10:37:42.1257252+03:00');

INSERT INTO [SessionData] ([Key], [Data]) VALUES
('Existing Data Key', '{"NumericProperty":12345,"StringProperty":"StringProperty From Database","CollectionProperty":["CollectionValue1 From Database","CollectionValue2 From Database"]}');
