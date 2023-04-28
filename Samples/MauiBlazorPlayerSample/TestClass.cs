using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Player;

namespace MauiBlazorPlayerSample
{
    internal class TestClass
    {
        public static IList<string> Mp3UrlList => new[]{
    "https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3",
    "https://cdn.pixabay.com/download/audio/2022/03/10/audio_e1ed0d3234.mp3?filename=mgs-first-one-my-own-version-of-music-by-kris-demo-sample-done-on-ableton-live-and-made-up-31742.mp3"
    };
        public TestClass()
        {
            CrossMediaManager.Current.Init();
            //CrossMediaManager.Current.AutoPlay = false;
            CrossMediaManager.Current.RepeatMode = MediaManager.Playback.RepeatMode.All;
            CrossMediaManager.Current.MediaItemChanged += Current_MediaItemChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            CrossMediaManager.Current.PropertyChanged += Current_PropertyChanged;
            CrossMediaManager.Current.MediaPlayer.BeforePlaying += MediaPlayer_BeforePlaying;
            CrossMediaManager.Current.Play(Mp3UrlList);
            //CrossMediaManager.Current.Play();
        }

        private void MediaPlayer_BeforePlaying(object sender, MediaManager.Player.MediaPlayerEventArgs e)
        {
            Debug.WriteLine($"Before playing {e.MediaItem.Id}");
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if(e.PropertyName != "PreviousPosition")
                //Debug.WriteLine($"Change {e.PropertyName}");
        }

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            if (e.State == MediaPlayerState.Playing)
            {
                Thread.Sleep(10000);
                var duration = ((MediaManagerBase)CrossMediaManager.Current).Duration.TotalSeconds;
                CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(duration*0.98));
            }
        }

        private void Current_MediaItemChanged(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            Debug.WriteLine($"New media event for: {e.MediaItem.Id}");
        }
    }
}
