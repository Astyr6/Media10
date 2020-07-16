using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Video10.Views
{
    public sealed partial class MediaPlayerPage : Page, INotifyPropertyChanged
    {
        // The DisplayRequest is used to stop the screen dimming while watching for extended periods
        private DisplayRequest _displayRequest = new DisplayRequest();
        private bool _isRequestActive = false;

        public MediaPlayerPage()
        {
            InitializeComponent();
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
            var mediaSource = mpe.Source as MediaSource;
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

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;

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
            } catch { }

        }

        private void AppBarButton_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mpe.MediaPlayer.Source = null;
        }

        private void playPause_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Convert.ToString(mpe.MediaPlayer.PlaybackSession.PlaybackState) == "Playing")
                mpe.MediaPlayer.Pause();
            else
                mpe.MediaPlayer.Play();
        }

        private void fullScreen_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (mpe.IsFullWindow)
                mpe.IsFullWindow = false;
            else
                mpe.IsFullWindow = true;
        }

        private void VolumeDown_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!(mpe.MediaPlayer.Volume == 0))
            {
                mpe.MediaPlayer.Volume = mpe.MediaPlayer.Volume - 0.01;
            }
        }

        private void VolumeUp_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!(mpe.MediaPlayer.Volume == 1))
                mpe.MediaPlayer.Volume = mpe.MediaPlayer.Volume + 0.01;
        }

        private void VolumeMute_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (mpe.MediaPlayer.IsMuted)
                mpe.MediaPlayer.IsMuted = false;
            else
                mpe.MediaPlayer.IsMuted = true;
        }
    }
}
