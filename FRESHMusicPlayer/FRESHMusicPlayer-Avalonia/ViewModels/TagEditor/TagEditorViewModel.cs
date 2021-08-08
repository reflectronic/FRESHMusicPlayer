﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATL;
using Avalonia.Controls;
using FRESHMusicPlayer.Handlers;
using ReactiveUI;

namespace FRESHMusicPlayer.ViewModels.TagEditor
{
    public class TagEditorViewModel : ViewModelBase
    {
        public Window Window { get; set; }
        public Player Player { get; set; }
        public Library Library { get; set; }

        private string artist;
        public string Artist
        {
            get => artist;
            set
            {
                this.RaiseAndSetIfChanged(ref artist, value);
                UnsavedChanges = true;
            }
        }
        private string title;
        public string Title
        {
            get => title;
            set 
            {
                this.RaiseAndSetIfChanged(ref title, value);
                UnsavedChanges = true;
            }
        }
        private string year;
        public string Year
        {
            get => year;
            set 
            {
                this.RaiseAndSetIfChanged(ref year, value);
                UnsavedChanges = true;
            }
        }
        private string genre;
        public string Genre
        {
            get => genre;
            set 
            {
                this.RaiseAndSetIfChanged(ref genre, value);
                UnsavedChanges = true;
            }
        }
        private string album;
        public string Album
        {
            get => album;
            set 
            {
                this.RaiseAndSetIfChanged(ref album, value);
                UnsavedChanges = true;
            }
        }

        private string albumArtist;
        public string AlbumArtist
        {
            get => albumArtist;
            set 
            {
                this.RaiseAndSetIfChanged(ref albumArtist, value);
                UnsavedChanges = true;
            }
        }
        private string composer;
        public string Composer
        {
            get => composer;
            set 
            {
                this.RaiseAndSetIfChanged(ref composer, value);
                UnsavedChanges = true;
            }
        }
        private string trackNumber;
        public string TrackNumber
        {
            get => trackNumber;
            set 
            {
                this.RaiseAndSetIfChanged(ref trackNumber, value);
                UnsavedChanges = true;
            }
        }
        private string discNumber;
        public string DiscNumber
        {
            get => discNumber;
            set
            {
                this.RaiseAndSetIfChanged(ref discNumber, value);
                UnsavedChanges = true;
            }
        }

        public List<string> FilePaths { get; set; } = new();

        private const string windowName = "FRESHMusicPlayer Tag Editor";
        public string WindowTitle
        {
            get
            {
                string initialString;
                if (FilePaths.Count > 0)
                {
                    initialString = $"{string.Join(", ", FilePaths)} | {windowName}";
                    if (UnsavedChanges) initialString = $"*{initialString}";
                    return initialString;
                }
                else
                {
                    return windowName;
                }
            }
        }

        private bool unsavedChanges = false;
        public bool UnsavedChanges
        {
            get => unsavedChanges;
            set
            {
                this.RaiseAndSetIfChanged(ref unsavedChanges, value);
                this.RaisePropertyChanged(nameof(WindowTitle));
            }
        }
        private bool isBackgroundSaveNeeded = false;
        public bool IsBackgroundSaveNeeded
        {
            get => isBackgroundSaveNeeded;
            set => this.RaiseAndSetIfChanged(ref isBackgroundSaveNeeded, value);
        }

        public void Initialize(List<string> filePaths)
        {
            FilePaths = filePaths;
            this.RaisePropertyChanged(nameof(WindowTitle));
            foreach (var path in filePaths)
            {
                var track = new Track(path);
                Artist = track.Artist;
                Title = track.Title;
                Album = track.Album;
                Genre = track.Genre;
                Year = track.Year.ToString();

                AlbumArtist = track.Album;
                Composer = track.Composer;
                TrackNumber = track.TrackNumber.ToString();
                DiscNumber = track.DiscNumber.ToString();
            }
            UnsavedChanges = false; // override setting these usually making unsaved changes true, kinda jank but it works lol
        }

        public void NewWindowCommand()
        {

        }

        private List<string> acceptableFilePaths = "wav;aiff;mp3;wma;3g2;3gp;3gp2;3gpp;asf;wmv;aac;adts;avi;m4a;m4a;m4v;mov;mp4;sami;smi;flac".Split(';').ToList();
        public async void OpenCommand()
        {
            var dialog = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter()
                    {
                        Name = "Audio Files",
                        Extensions = acceptableFilePaths
                    },
                    new FileDialogFilter()
                    {
                        Name = "Other",
                        Extensions = new List<string>() { "*" }
                    }
                },
                AllowMultiple = true
            };
            var files = await dialog.ShowAsync(GetMainWindow());
            Initialize(files.ToList());
        }

        private List<string> filePathsToSaveInBackground = new();
        public void SaveCommand()
        {
            foreach (string path in FilePaths)
            {
                if (path != Player.FilePath) continue;
                else
                {
                    filePathsToSaveInBackground.AddRange(FilePaths);
                    IsBackgroundSaveNeeded = true;
                }
            }

            foreach (string path in FilePaths)
            {
                var track = new Track(path)
                {
                    Artist = Artist,
                    Title = Title,
                    Album = Album,
                    Genre = Genre,
                    Year = Convert.ToInt32(Year),
                    AlbumArtist = AlbumArtist,
                    Composer = Composer,
                    TrackNumber = Convert.ToInt32(TrackNumber),
                    DiscNumber = Convert.ToInt32(DiscNumber)
                };
                track.Save();
                Library?.Remove(path); // update library entry, if available
                Library?.Import(path);
            }
        }

        public void ExitCommand()
        {

        }
    }
}
