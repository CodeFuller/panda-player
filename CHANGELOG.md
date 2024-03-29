# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 4.3.1 - 2023-10-14
- Replaced package MvvmLightLibsStd10 with CommunityToolkit.Mvvm.
- Updated package CodeFuller.Library.Wpf to version 8.2.0.

## 4.3.0 - 2023-10-01
- Implemented update of songs content.
- Upgraded to .NET 7.0.

## 4.2.0 - 2023-05-09
- Added support of verbatim titles in reference content.

## 4.1.1 - 2022-11-26
- Fixed checking of song titles with dot character.

## 4.1.0 - 2022-02-12
Fixes and improvements in disc adder:
- Fixed in-place edit of actual content.
- Applied Material Design styles for tree views.
- Added separate reload of different content types - reference, actual and all.
- Fixed re-editing of items in actual content tree.
- Removed broken double-click in actual content tree.
- Fixed submit of unchanged disc directory path.
- Improved performance of reference content update.
- Added Close button after content was added to the library.
- Added clearing of reference content after adding to the library.
- Prevented re-adding of the same content.
- Allowed navigating back from adding to library page.
- Prevented adding of empty discs.

## 4.0.0 - 2022-01-31
- Upgraded to .NET 6.0.
- Switched to fully loaded library with single model instances.
- Fixed playbacks discrepancy error for songs from different sources (e.g. library explorer and adviser).

## 3.19.0 - 2021-11-21
- Added the ability to rename folders.

## 3.18.0 - 2021-10-26
- Added display of deleted content.
- Added ability to edit comment for previously deleted discs and songs.
- Improved context menu so that only applicable commands are shown.
- Updated "Image Not Found" stub picture.
- Fixed first open of context menu.

## 3.17.1 - 2021-10-16
- Added icons for context menu items.

## 3.17.0 - 2021-10-16
- Added toolbar.

## 3.16.1 - 2021-10-15
- Added multi-line mode for delete comment text box.
- Fixed resizing of dialog windows according to input content.
- Fixed clearing of previous comment in delete content dialog.
- Fixed wrap of long confirmation message in delete content dialog.

## 3.16.0 - 2021-10-15
- Added support of delete comment.

## 3.15.1 - 2021-10-11
- Fixed double click on unselected folder in library explorer.

## 3.15.0 - 2021-10-10
- Replaced favorite artists with favorite advise groups.

## 3.14.3 - 2021-10-09
- Fixed navigation to current disc for advise set playlist.

## 3.14.2 - 2021-10-08
- Fixed setting of blank artist.

## 3.14.1 - 2021-10-07
- Fixed creation of new artist with partially matching name.

## 3.14.0 - 2021-10-03
- Extended song properties dialog with ability to create new artists.

## 3.13.0 - 2021-10-02
- Added advise sets.

## 3.12.0 - 2021-08-20
- Extended library explorer with item icons.
- Disabled multi-selection mode for library explorer.
- Removed border & focus for selection in DataGrid controls.
- Fixed Material Design styles for DataGrid controls.

## 3.11.1 - 2021-08-19
- Fixed display of current song time (2:58 / 0:00) when playlist is finished.

## 3.11.0 - 2021-08-18
- Added advise groups for folders and discs.

## 3.10.4 - 2021-08-09
- Fixed display of current song time when playback is paused.

## 3.10.3 - 2021-08-08
- Changed icons for adviser buttons.

## 3.10.2 - 2021-07-26
- Fixed update of disc tree title in library explorer.

## 3.10.1 - 2021-07-25
- Fixed ordering for explorer items starting from the letter 'Ё' in Russian.

## 3.10.0 - 2021-07-14
- Added the ability to add a disc to the playlist.
- Fixed "Play From This Song" menu item.

## 3.9.0 - 2021-07-11
- Added following items to the context menu for playlist:
  - Play from this song
  - Remove from playlist
  - Clear playlist
  - Go to disc
- Fixed loading of saved playlist with duplicated songs ("Index of current song in saved playlist is invalid")

## 3.8.2 - 2021-07-04
- Fixed deletion of disc cover images from the database when disc is deleted.

## 3.8.1 - 2021-06-20
- Added congiguration of API URL for Last.FM scrobbler.
- Switched API URL for Last.FM scrobbler from http to https.

## 3.8.0 - 2021-01-24
- Upgraded to .NET 5.0.
- Updated CodeFuller.Library to version 7.2.0.
- Added MIT license.

## 3.7.2 - 2020-07-23
- Fixed startup error of loading songs playlist.

## 3.7.1 - 2020-06-15
- Improved estimation of library check progress.

## 3.7.0 - 2020-06-14
- Extended library checker with check for tags consistency.

## 3.6.3 - 2020-06-14
- Unified tree title for disc cover images set by disc adder.

## 3.6.2 - 2020-06-14
- Fixed synchronization of songs deletion between lists.

## 3.6.1 - 2020-06-14
- Fixed loading of content explorer when playlist songs belong to various discs.

## 3.6.0 - 2020-06-14
- Moved session data to the database.

## 3.5.1 - 2020-06-13
- Fixed disc adder for case when disc has no cover image.

## 3.5.0 - 2020-06-13
- Made filling of details for new discs more flexible.

## 3.4.2 - 2020-06-11
- Fixed loading of last playlist if some songs are missing in the library.

## 3.4.1 - 2020-06-11
- Moved year property from song to disc.

## 3.4.0 - 2020-06-10
- Integrated disc adder.

## 3.3.0 - 2020-06-07
- Added library checker.

## 3.2.2 - 2020-06-06
- Extended context menu for disc with Play Disc command.

## 3.2.1 - 2020-06-06
- Restored editing of disc properties.

## 3.2.0 - 2020-06-05
- Implemented deletion for empty folders.

## 3.1.11 - 2020-06-05
- Fixed ordering of disc songs.

## 3.1.10 - 2020-06-05
- Added deletion of empty disc directory when disc is deleted.

## 3.1.9 - 2020-06-04
- Added stub disc cover image when image file is missing.

## 3.1.8 - 2020-06-04
- Removed unnecessary loading of root folder on startup.

## 3.1.7 - 2020-06-04
- Unified buttons width in dialog windows.

## 3.1.6 - 2020-06-04
- Removed left border for first column in songs list.

## 3.1.5 - 2020-06-03
- Disabled changing of track number when multiple songs are edited.

## 3.1.4 - 2020-06-03
- Fixed filling of song properties lists when multiple songs are edited.

## 3.1.3 - 2020-06-02
- Centered header for library content explorer.

## 3.1.2 - 2020-06-02
- Fixed placement of tooltip for icon in system tray.

## 3.1.1 - 2020-06-02
- Fixed width of statistics window.

## 3.1.0 - 2020-06-02
- Improved UI layout.

## 3.0.6 - 2020-05-30
- Fixed colors for combo boxes in song properties window.

## 3.0.5 - 2020-05-30
- Decreased bonus for rating in adviser.

## 3.0.4 - 2020-05-30
- Fixed display of context menu.

## 3.0.3 - 2020-05-30
- Hid deleted folders from library explorer.

## 3.0.2 - 2020-05-30
- Fixed synchronization of disc and song changes between views.

## 3.0.1 - 2020-05-30
- Fixed timestamp of Last&#46;FM scrobbles.

## 3.0.0 - 2020-05-29
- Switched from in-memory DiscLibrary to service operations.

## 2.0.0 - 2020-04-18
- Upgraded to .NET Core 3.1.
