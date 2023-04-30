using MediaManager.Library;
using MediaManager.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MediaManager.Platforms.Uap.Media
{
    public static class MediaItemExtensions
    {
        public static async Task<MediaSource> ToMediaSource(this IMediaItem mediaItem)
        {
            MediaSource res = null;
            var PlayListMediaItem = mediaItem;
            if (mediaItem.MediaLocation.IsLocal())
            {
                var storageFile = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);
                res = MediaSource.CreateFromStorageFile(storageFile);
            }
            else if (mediaItem.MediaLocation == MediaLocation.InMemory)
            {
                res = MediaSource.CreateFromStream(mediaItem.Data.AsRandomAccessStream(), mediaItem.MimeType.ToMimeTypeString());
            }
            else
            {
                res = MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
            }
            if (PlayListMediaItem is not null)
            {
                res.CustomProperties["Id"] = PlayListMediaItem.Id;
                res.CustomProperties["Album"] = PlayListMediaItem.Album;
                res.CustomProperties["DisplayTitle"] = PlayListMediaItem.DisplayTitle;
                res.CustomProperties["Artist"] = PlayListMediaItem.Artist;
                res.CustomProperties["TrackNumber"] = PlayListMediaItem.TrackNumber;
                res.CustomProperties["Thumbnail"] = PlayListMediaItem.AlbumImageUri;
            }
            return res;
        }

        public static MediaPlaybackItem ToMediaPlaybackItem(this MediaSource mediaSource)
        {
            MediaPlaybackItem res;
            res = new MediaPlaybackItem(mediaSource);
            var displayProperties = res.GetDisplayProperties();
            displayProperties.Type = Windows.Media.MediaPlaybackType.Music;
            displayProperties.MusicProperties.AlbumTitle = (string)mediaSource.CustomProperties["Album"];
            displayProperties.MusicProperties.Title = (string)mediaSource.CustomProperties["DisplayTitle"];
            displayProperties.MusicProperties.Artist = (string)mediaSource.CustomProperties["Artist"];
            displayProperties.MusicProperties.TrackNumber = uint.Parse(mediaSource.CustomProperties["TrackNumber"].ToString());
            displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri((string)mediaSource.CustomProperties["Thumbnail"]));
            res.ApplyDisplayProperties(displayProperties);
            return res;
        }

        public static MediaPlaybackItem ToMediaPlaybackItem(this MediaSource mediaSource, TimeSpan startAt, TimeSpan? stopAt = null)
        {
            MediaPlaybackItem res;
            if (stopAt is TimeSpan endTime)
                res = new MediaPlaybackItem(mediaSource, startAt, endTime);

            res = new MediaPlaybackItem(mediaSource, startAt);
            var displayProperties = res.GetDisplayProperties();
            displayProperties.MusicProperties.AlbumTitle = (string)mediaSource.CustomProperties["Album"];
            displayProperties.MusicProperties.Title = (string)mediaSource.CustomProperties["DisplayTitle"];
            displayProperties.MusicProperties.Artist = (string)mediaSource.CustomProperties["Artist"];
            displayProperties.MusicProperties.TrackNumber = uint.Parse(mediaSource.CustomProperties["TrackNumber"].ToString());
            displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri((string)mediaSource.CustomProperties["Thumbnail"]));
            res.ApplyDisplayProperties(displayProperties);

            return res;
        }
    }
}
