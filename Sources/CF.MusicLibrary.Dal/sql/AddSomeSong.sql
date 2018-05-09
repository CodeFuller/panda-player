INSERT INTO Genres(Name) VALUES('Bard');

INSERT INTO Artists(Name) VALUES('Some Artist');

INSERT INTO Discs(Year, Title, Albumtitle, Uri) VALUES(2017, 'Some Disc', 'Some Album', '/Foreign/Some Artist/Some Disc');

INSERT INTO songs(TrackNumber, Year, Title, Duration, Rating, Uri, FileSize, Bitrate, LastPlaybackTime, PlaybacksCount, Artist_Id, Disc_Id, Genre_Id)
  VALUES(1, 2017, 'Some Song', 123, NULL, '/Foreign/Some Artist/Some Disc/01 - Some Song.mp3', 12345, 320000, NULL, 0, 1, 1, 1);
