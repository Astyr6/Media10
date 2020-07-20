using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;

namespace Video10.Views
{
    public sealed partial class MediaPlayerPage : Page, INotifyPropertyChanged
    {
        // The DisplayRequest is used to stop the screen dimming while watching for extended periods
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private bool _isRequestActive = false;

        public MediaPlayerPage()
        {
            InitializeComponent();
            BackButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            ShuffleButton.IsEnabled = false;
            RepeatButton.IsEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            mpe.MediaPlayer.Pause();
            mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
            MediaSource mediaSource = mpe.Source as MediaSource;
            mediaSource?.Dispose();
            mpe.Source = null;
        }

        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (sender is MediaPlaybackSession playbackSession && playbackSession.NaturalVideoHeight != 0)
            {
                if (playbackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    if (!_isRequestActive)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestActive();
                            _isRequestActive = true;
                        });
                    }
                }
                else
                {
                    if (_isRequestActive)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestRelease();
                            _isRequestActive = false;
                        });
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
            };

            picker.FileTypeFilter.Add(".asf");
            picker.FileTypeFilter.Add(".wma");
            picker.FileTypeFilter.Add(".wmv");
            picker.FileTypeFilter.Add(".wm");

            picker.FileTypeFilter.Add(".asx");
            picker.FileTypeFilter.Add(".wax");
            picker.FileTypeFilter.Add(".wvx");
            picker.FileTypeFilter.Add(".wmx");
            picker.FileTypeFilter.Add(".wpl");

            picker.FileTypeFilter.Add(".dvr-ms");

            picker.FileTypeFilter.Add(".wmd");

            picker.FileTypeFilter.Add(".avi");

            picker.FileTypeFilter.Add(".mpg");
            picker.FileTypeFilter.Add(".mpeg");
            picker.FileTypeFilter.Add(".m1v");
            picker.FileTypeFilter.Add(".mp2");
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".mpa");
            picker.FileTypeFilter.Add(".mpe");
            picker.FileTypeFilter.Add(".m3u");

            picker.FileTypeFilter.Add(".mid");
            picker.FileTypeFilter.Add(".midi");
            picker.FileTypeFilter.Add(".rmi");

            picker.FileTypeFilter.Add(".aif");
            picker.FileTypeFilter.Add(".aifc");
            picker.FileTypeFilter.Add(".aiff");

            picker.FileTypeFilter.Add(".au");
            picker.FileTypeFilter.Add(".snd");

            picker.FileTypeFilter.Add(".wav");

            picker.FileTypeFilter.Add(".cda");

            picker.FileTypeFilter.Add(".ivf");

            picker.FileTypeFilter.Add(".m4a");

            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".m4v");
            picker.FileTypeFilter.Add(".mp4v");
            picker.FileTypeFilter.Add(".3g2");
            picker.FileTypeFilter.Add(".3gp2");
            picker.FileTypeFilter.Add(".3gp");
            picker.FileTypeFilter.Add(".3gpp");

            picker.FileTypeFilter.Add(".aac");
            picker.FileTypeFilter.Add(".adt");
            picker.FileTypeFilter.Add(".adts");

            picker.FileTypeFilter.Add(".m2ts");
            picker.FileTypeFilter.Add(".ts");

            picker.FileTypeFilter.Add(".flac");

            picker.FileTypeFilter.Add(".mkv");
            picker.FileTypeFilter.Add(".ogg");

            try
            {
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

                mpe.Source = MediaSource.CreateFromStorageFile(file);
            }
            catch { }

        }

        private void AppBarButton_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mpe.MediaPlayer.Source = null;
        }

        private void playPause_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Convert.ToString(mpe.MediaPlayer.PlaybackSession.PlaybackState) == "Playing")
            {
                mpe.MediaPlayer.Pause();
            }
            else
            {
                mpe.MediaPlayer.Play();
            }
        }

        private void fullScreen_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (mpe.IsFullWindow)
            {
                mpe.IsFullWindow = false;
            }
            else
            {
                mpe.IsFullWindow = true;
            }
        }

        private void VolumeDown_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!(mpe.MediaPlayer.Volume > 0.01))
            {
                mpe.MediaPlayer.Volume = mpe.MediaPlayer.Volume - 0.01;
            }
            if (mpe.MediaPlayer.Volume == 0.01)
            {
                mpe.MediaPlayer.Volume = 0.00;
                mpe.MediaPlayer.IsMuted = true;
            }
        }

        private void VolumeUp_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!(mpe.MediaPlayer.Volume == 1))
            {
                mpe.MediaPlayer.IsMuted = false;
                mpe.MediaPlayer.Volume = mpe.MediaPlayer.Volume + 0.01;
            }
        }

        private void VolumeMute_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (mpe.MediaPlayer.IsMuted)
            {
                mpe.MediaPlayer.IsMuted = false;
            }
            else
            {
                mpe.MediaPlayer.IsMuted = true;
            }
        }

        private void escapeFullscreen_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mpe.IsFullWindow = false;
        }

        private void Grid_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.DragUIOverride.Caption = "Drop to play";
            e.AcceptedOperation = DataPackageOperation.Link;
            SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            playlistListView.Background = grayBrush;
        }

        private async void Grid_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                if (items.Count == 1)
                {
                    StorageFile storageFile = items[0] as StorageFile;
                    string[] allowedTypes = { ".asf", ".wma", ".wmv", ".wm", ".asx", ".wax", ".wvx", ".wmx", ".wpl", ".dvr-ms", ".wmd", ".avi", ".mpg", ".mpeg", ".m1v", ".mp2", ".mp3", ".mpa", ".mpe", ".m3u", ".mid", ".midi", ".rmi", ".aif", ".aifc", ".aiff", ".au", ".snd", ".wav", ".cda", ".ivf", ".m4a", ".mp4", ".m4v", ".mp4v", ".3g2", ".3gp2", ".3gp", ".3gpp", ".aac", ".adt", ".adts", ".m2ts", ".ts", ".flac", ".mkv", ".ogg" };
                    foreach (string x in allowedTypes)
                    {
                        if (storageFile.FileType.Contains(x))
                        {
                            mpe.Source = MediaSource.CreateFromStorageFile(storageFile);
                        }
                    }
                }
            }
        }

        private readonly string[] playlist = { };

        private async void AppBarButton_Click_2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await playlistDialog.ShowAsync();
        }

        private void playlistGrid_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.DragUIOverride.Caption = "Drop to add to the playlist";
            e.AcceptedOperation = DataPackageOperation.Link;
        }


        
        private async void playlistGrid_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                foreach (IStorageItem items1 in items)
                {
                    if (items.Count > 0)
                    {
                        StorageFile storageFile = items1 as StorageFile;
                        string[] allowedTypes = { ".asf", ".wma", ".wmv", ".wm", ".asx", ".wax", ".wvx", ".wmx", ".wpl", ".dvr-ms", ".wmd", ".avi", ".mpg", ".mpeg", ".m1v", ".mp2", ".mp3", ".mpa", ".mpe", ".m3u", ".mid", ".midi", ".rmi", ".aif", ".aifc", ".aiff", ".au", ".snd", ".wav", ".cda", ".ivf", ".m4a", ".mp4", ".m4v", ".mp4v", ".3g2", ".3gp2", ".3gp", ".3gpp", ".aac", ".adt", ".adts", ".m2ts", ".ts", ".flac", ".mkv", ".ogg" };
                        foreach (string x in allowedTypes)
                        {
                            if (storageFile.FileType.Contains(x))
                            {
                                playlistListView.Items.Add(storageFile);
                            }
                        }
                    }

                    if (playlistListView.Items.Count > 0)
                    {
                        DragAndDropPormpt.Visibility = Visibility.Collapsed;
                        BackButton.IsEnabled = true;
                        NextButton.IsEnabled = true;
                        ShuffleButton.IsEnabled = true;
                        RepeatButton.IsEnabled = true;
                    }
                    else if (playlistListView.Items.Count == 0)
                    {
                        DragAndDropPormpt.Visibility = Visibility.Visible;
                        BackButton.IsEnabled = false;
                        NextButton.IsEnabled = false;
                        ShuffleButton.IsEnabled = false;
                        RepeatButton.IsEnabled = false;
                    }
                }
                
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try { playlistListView.Items.RemoveAt(playlistListView.SelectedIndex); } catch { }
            if (playlistListView.Items.Count == 0)
            {
                DragAndDropPormpt.Visibility = Visibility.Visible;
                BackButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                ShuffleButton.IsEnabled = false;
                RepeatButton.IsEnabled = false;
            }

        }

        private void playlistListView_DragLeave(object sender, DragEventArgs e)
        {
            SolidColorBrush transparentBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            playlistListView.Background = transparentBrush;
        }



        List<PlaylistItem> PlaylistList = new List<PlaylistItem>();
        MediaPlaybackList _playbackList = new MediaPlaybackList();
        private void PlaylistSystem()
        {
            PlaylistList.Clear();
            for (int i = 0; i < playlistListView.Items.Count; i++)
            {
                PlaylistList.Add(new PlaylistItem((StorageFile)playlistListView.Items[i]));
            }

            _playbackList.MaxPlayedItemsToKeepOpen = 2;
            
            foreach (var st in PlaylistList)
            {
                var mediaPlaybackItem = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(st.File));
                _playbackList.Items.Add(mediaPlaybackItem);
            }
            mpe.MediaPlayer.Source = _playbackList;
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (playlistListView.Items.Count != 0)
            PlaylistSystem();
        }

        public class PlaylistItem
        {
            private StorageFile file;

            public PlaylistItem(StorageFile file)
            {
                this.file = file;
                //this.index = index;
            }

            public StorageFile File
            {
                get { return file; }
                set { file = value; }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _playbackList.MovePrevious();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _playbackList.MoveNext();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShuffleButton.IsChecked == true)
                _playbackList.ShuffleEnabled = true;
            else if (ShuffleButton.IsChecked == false)
                _playbackList.ShuffleEnabled = false;
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (RepeatButton.IsChecked == true)
                _playbackList.AutoRepeatEnabled = true;
            else if (RepeatButton.IsChecked == false)
                _playbackList.AutoRepeatEnabled = false;
        }

        private void Button_Click_1(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (playlistListView.Items.Count > 0)
            {
                PlaylistSystem();
                playlistDialog.Hide();
            }

        }

        private async void Open_File_Playlist_Button_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
            };

            picker.FileTypeFilter.Add(".asf");
            picker.FileTypeFilter.Add(".wma");
            picker.FileTypeFilter.Add(".wmv");
            picker.FileTypeFilter.Add(".wm");

            picker.FileTypeFilter.Add(".asx");
            picker.FileTypeFilter.Add(".wax");
            picker.FileTypeFilter.Add(".wvx");
            picker.FileTypeFilter.Add(".wmx");
            picker.FileTypeFilter.Add(".wpl");

            picker.FileTypeFilter.Add(".dvr-ms");

            picker.FileTypeFilter.Add(".wmd");

            picker.FileTypeFilter.Add(".avi");

            picker.FileTypeFilter.Add(".mpg");
            picker.FileTypeFilter.Add(".mpeg");
            picker.FileTypeFilter.Add(".m1v");
            picker.FileTypeFilter.Add(".mp2");
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".mpa");
            picker.FileTypeFilter.Add(".mpe");
            picker.FileTypeFilter.Add(".m3u");

            picker.FileTypeFilter.Add(".mid");
            picker.FileTypeFilter.Add(".midi");
            picker.FileTypeFilter.Add(".rmi");

            picker.FileTypeFilter.Add(".aif");
            picker.FileTypeFilter.Add(".aifc");
            picker.FileTypeFilter.Add(".aiff");

            picker.FileTypeFilter.Add(".au");
            picker.FileTypeFilter.Add(".snd");

            picker.FileTypeFilter.Add(".wav");

            picker.FileTypeFilter.Add(".cda");

            picker.FileTypeFilter.Add(".ivf");

            picker.FileTypeFilter.Add(".m4a");

            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".m4v");
            picker.FileTypeFilter.Add(".mp4v");
            picker.FileTypeFilter.Add(".3g2");
            picker.FileTypeFilter.Add(".3gp2");
            picker.FileTypeFilter.Add(".3gp");
            picker.FileTypeFilter.Add(".3gpp");

            picker.FileTypeFilter.Add(".aac");
            picker.FileTypeFilter.Add(".adt");
            picker.FileTypeFilter.Add(".adts");

            picker.FileTypeFilter.Add(".m2ts");
            picker.FileTypeFilter.Add(".ts");

            picker.FileTypeFilter.Add(".flac");

            picker.FileTypeFilter.Add(".mkv");
            picker.FileTypeFilter.Add(".ogg");

            try
            {
                var files = await picker.PickMultipleFilesAsync();

                foreach (IStorageItem file in files)
                {
                    if (files.Count > 0)
                    {
                        StorageFile storageFile = file as StorageFile;
                        string[] allowedTypes = { ".asf", ".wma", ".wmv", ".wm", ".asx", ".wax", ".wvx", ".wmx", ".wpl", ".dvr-ms", ".wmd", ".avi", ".mpg", ".mpeg", ".m1v", ".mp2", ".mp3", ".mpa", ".mpe", ".m3u", ".mid", ".midi", ".rmi", ".aif", ".aifc", ".aiff", ".au", ".snd", ".wav", ".cda", ".ivf", ".m4a", ".mp4", ".m4v", ".mp4v", ".3g2", ".3gp2", ".3gp", ".3gpp", ".aac", ".adt", ".adts", ".m2ts", ".ts", ".flac", ".mkv", ".ogg" };
                        foreach (string x in allowedTypes)
                        {
                            if (storageFile.FileType.Contains(x))
                            {
                                playlistListView.Items.Add(storageFile);
                            }
                        }
                    }
                }
                if (playlistListView.Items.Count > 0)
                {
                    DragAndDropPormpt.Visibility = Visibility.Collapsed;
                    BackButton.IsEnabled = true;
                    NextButton.IsEnabled = true;
                    ShuffleButton.IsEnabled = true;
                    RepeatButton.IsEnabled = true;
                }
                else if (playlistListView.Items.Count == 0)
                {
                    DragAndDropPormpt.Visibility = Visibility.Visible;
                    BackButton.IsEnabled = false;
                    NextButton.IsEnabled = false;
                    ShuffleButton.IsEnabled = false;
                    RepeatButton.IsEnabled = false;
                }
            }
            catch { }
        }

        private void Clear_Playlist_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            { playlistListView.Items.Clear(); } catch { }
            if (playlistListView.Items.Count == 0)
            {
                DragAndDropPormpt.Visibility = Visibility.Visible;
                BackButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                ShuffleButton.IsEnabled = false;
                RepeatButton.IsEnabled = false;
            }
        }

        private void playlistDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            try { playlistListView.SelectedItem = _playbackList.CurrentItemIndex; } catch { }
        }
    }
}
