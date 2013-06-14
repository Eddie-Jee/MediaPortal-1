#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Text;
using System.Web;
using MediaPortal.TagReader;
using MediaPortal.Playlists;

namespace MediaPortal.Music.Database
{
  /// <summary>
  /// The audioscrobbler submit queue status
  /// </summary>
  public enum SongStatus
  {
    Init,
    Loaded,
    Cached,
    Queued,
    Submitted,
    Short
  }

  /// <summary>
  /// The source which "suggested" the track
  /// </summary>
  public enum SongSource
  {
    /// <summary>
    /// Chosen by the user
    /// </summary>
    P,
    /// <summary>
    /// Non-personalised broadcast (e.g. Shoutcast, BBC Radio 1).
    /// </summary>
    R,
    /// <summary>
    /// Personalised recommendation except Last.fm (e.g. Pandora, Launchcast).
    /// </summary>
    E,
    /// <summary>
    /// Last.fm (any mode). prove validity of submission by sourceID (for example, "o[0]=L1b48a").
    /// </summary>
    L,
    /// <summary>
    /// Source unknown
    /// </summary>
    U,
  }

  /// <summary>
  /// A single character denoting the rating of the track. Empty if not applicable
  /// </summary>
  public enum SongAction
  {
    /// <summary>
    /// None
    /// </summary>
    N,
    /// <summary>
    /// Love (on any mode if the user has manually loved the track). This implies a listen.
    /// </summary>
    L,
    /// <summary>
    /// Ban (only if source=L). This implies a skip, and the client should skip to the next track when a ban happens.
    /// </summary>
    B,
    /// <summary>
    /// Skip (only if source=L)
    /// </summary>
    S,
  }

  [Serializable()]
  public class Song
  {
    private string _strArtist = String.Empty;
    private int _iTrack = 0;
    private int _iNumTracks = 0;
    private int _iDuration = 0;
    private int _iYear = 0;
    private int _iResumeAt = 0;
    private int _iDisc = 0;
    private int _iNumDisc = 0;

    public Song()
    {
      AudioscrobblerAction = SongAction.N;
      AuthToken = string.Empty;
      Source = SongSource.P;
      LastFMMatch = string.Empty;
      WebImage = string.Empty;
      URL = string.Empty;
      Favorite = false;
      Id = -1;
      Album = string.Empty;
      AlbumSort = string.Empty;
      AlbumArtist = string.Empty;
      AlbumArtistSort = string.Empty;
      Artist = string.Empty;
      ArtistSort = string.Empty;
      AmazonId = string.Empty;
      BPM = 0;
      Comment = string.Empty;
      Composer = string.Empty;
      ComposerSort = string.Empty;
      Conductor = string.Empty;
      Copyright = string.Empty;
      DateTimePlayed = DateTime.MinValue;
      DateTimeModified = DateTime.MinValue;
      DiscTotal = 0;
      DiscId = 0;
      Genre = string.Empty;
      Grouping = string.Empty;
      Lyrics = string.Empty;
      MusicBrainzArtistId = string.Empty;
      MusicBrainzDiscId = string.Empty;
      MusicBrainzReleaseArtistId = string.Empty;
      MusicBrainzReleaseCountry = string.Empty;
      MusicBrainzReleaseId = string.Empty;
      MusicBrainzReleaseStatus = string.Empty;
      MusicBrainzReleaseTrackId = string.Empty;
      MusicBrainzReleaseType = string.Empty;
      MusicIpid = string.Empty;
      Rating = 0;
      ReplayGainAlbumPeak = string.Empty;
      ReplayGainAlbum = string.Empty;
      ReplayGainTrackPeak = string.Empty;
      ReplayGainTrack = string.Empty;
      Title = string.Empty;
      TitleSort = string.Empty;
      TimesPlayed = 0;
      TrackTotal = 0;
      Track = 0;
      Year = 0;
      BitRate = 0;
      BitRateMode = string.Empty;
      Channels = 0;
      Codec = string.Empty;
      Duration = 0;
      FileName = string.Empty;
      FileType = string.Empty;
      SampleRate = 0;
    }

    public Song Clone()
    {
      Song newsong = new Song();
      newsong.Id = Id;
      newsong.Album = Album;
      newsong.AlbumSort = AlbumSort;
      newsong.Artist = Artist;
      newsong.AlbumArtist = AlbumArtist;
      newsong.Duration = Duration;
      newsong.FileName = FileName;
      newsong.Genre = Genre;
      newsong.Grouping = Grouping;
      newsong.Composer = Composer;
      newsong.ComposerSort = ComposerSort;
      newsong.Conductor = Conductor;
      newsong.Copyright = Copyright;
      newsong.MusicBrainzArtistId = MusicBrainzArtistId;
      newsong.MusicBrainzDiscId = MusicBrainzDiscId;
      newsong.MusicBrainzReleaseArtistId = MusicBrainzReleaseArtistId;
      newsong.MusicBrainzReleaseCountry = MusicBrainzReleaseCountry;
      newsong.MusicBrainzReleaseId = MusicBrainzReleaseId;
      newsong.MusicBrainzReleaseStatus = MusicBrainzReleaseStatus;
      newsong.MusicBrainzReleaseTrackId = MusicBrainzReleaseTrackId;
      newsong.MusicBrainzReleaseType = MusicBrainzReleaseType;
      newsong.MusicIpid = MusicIpid;
      newsong.TimesPlayed = TimesPlayed;
      newsong.Title = Title;
      newsong.TitleSort = TitleSort;
      newsong.Track = Track;
      newsong.TrackTotal = TrackTotal;
      newsong.Year = Year;
      newsong.Rating = Rating;
      newsong.ReplayGainAlbum = ReplayGainAlbum;
      newsong.ReplayGainAlbumPeak = ReplayGainAlbumPeak;
      newsong.ReplayGainTrack = ReplayGainTrack;
      newsong.ReplayGainTrackPeak = ReplayGainTrackPeak;
      newsong.Favorite = Favorite;
      newsong.DateTimeModified = DateTimeModified;
      newsong.DateTimePlayed = DateTimePlayed;
      newsong.AudioScrobblerStatus = AudioScrobblerStatus;
      newsong.URL = URL;
      newsong.WebImage = WebImage;
      newsong.LastFMMatch = LastFMMatch;
      newsong.ResumeAt = ResumeAt;
      newsong.DiscId = DiscId;
      newsong.DiscTotal = DiscTotal;
      newsong.Lyrics = Lyrics;
      newsong.Source = Source;
      newsong.AuthToken = AuthToken;
      newsong.AudioscrobblerAction = AudioscrobblerAction;
      newsong.Comment = Comment;
      newsong.FileType = FileType;
      newsong.Codec = Codec;
      newsong.BitRateMode = BitRateMode;
      newsong.BPM = BPM;
      newsong.Channels = Channels;
      newsong.SampleRate = SampleRate;

      return newsong;
    }

    public void Clear()
    {
      AudioscrobblerAction = SongAction.N;
      AuthToken = string.Empty;
      Source = SongSource.P;
      LastFMMatch = string.Empty;
      WebImage = string.Empty;
      URL = string.Empty;
      Favorite = false;
      Id = -1;
      Album = string.Empty;
      AlbumSort = string.Empty;
      AlbumArtist = string.Empty;
      AlbumArtistSort = string.Empty;
      Artist = string.Empty;
      ArtistSort = string.Empty;
      AmazonId = string.Empty;
      BPM = 0;
      Comment = string.Empty;
      Composer = string.Empty;
      ComposerSort = string.Empty;
      Conductor = string.Empty;
      Copyright = string.Empty;
      DateTimePlayed = DateTime.MinValue;
      DateTimeModified = DateTime.MinValue;
      DiscTotal = 0;
      DiscId = 0;
      Genre = string.Empty;
      Grouping = string.Empty;
      Lyrics = string.Empty;
      MusicBrainzArtistId = string.Empty;
      MusicBrainzDiscId = string.Empty;
      MusicBrainzReleaseArtistId = string.Empty;
      MusicBrainzReleaseCountry = string.Empty;
      MusicBrainzReleaseId = string.Empty;
      MusicBrainzReleaseStatus = string.Empty;
      MusicBrainzReleaseTrackId = string.Empty;
      MusicBrainzReleaseType = string.Empty;
      MusicIpid = string.Empty;
      Rating = 0;
      ReplayGainAlbumPeak = string.Empty;
      ReplayGainAlbum = string.Empty;
      ReplayGainTrackPeak = string.Empty;
      ReplayGainTrack = string.Empty;
      Title = string.Empty;
      TitleSort = string.Empty;
      TimesPlayed = 0;
      TrackTotal = 0;
      Track = 0;
      Year = 0;
      BitRate = 0;
      BitRateMode = string.Empty;
      Channels = 0;
      Codec = string.Empty;
      Duration = 0;
      FileName = string.Empty;
      FileType = string.Empty;
      SampleRate = 0;
    }

    #region Getters & Setters

    public int Id { get; set; }

    public string FileName { get; set; }

    public string Artist
    {
      get { return _strArtist; }
      set
      {
        _strArtist = value;
        //remove 01. artist name
        if (_strArtist.Length > 4)
        {
          if (Char.IsDigit(_strArtist[0]) &&
              Char.IsDigit(_strArtist[1]) &&
              _strArtist[2] == '.' &&
              _strArtist[3] == ' ')
          {
            _strArtist = _strArtist.Substring(4);
          }
        }
        //remove artist name [dddd]
        int pos = _strArtist.IndexOf("[");
        if (pos > 0)
        {
          _strArtist = _strArtist.Substring(pos);
        }
        _strArtist = _strArtist.Trim();
      }
    }

    public string ArtistSort { get; set; }

    public string AlbumArtist { get; set; }

    public string AlbumArtistSort { get; set; }

    public string Album { get; set; }

    public string AlbumSort { get; set; }

    public string AmazonId { get; set; }

    public string Genre { get; set; }

    public string Grouping { get; set; }

    public string Composer { get; set; }

    public string ComposerSort { get; set; }

    public string Copyright { get; set; }

    public string Conductor { get; set; }

    public string Title { get; set; }

    public string TitleSort { get; set; }

    /// <summary>
    /// Track number
    /// </summary>
    public int Track
    {
      get { return _iTrack; }
      set
      {
        _iTrack = value;
        if (_iTrack < 0)
        {
          _iTrack = 0;
        }
      }
    }

    public int TrackTotal
    {
      get { return _iNumTracks; }
      set
      {
        _iNumTracks = value;
        if (_iNumTracks < 0)
        {
          _iNumTracks = 0;
        }
      }
    }

    /// <summary>
    /// Length of song in total seconds
    /// </summary>
    public int Duration
    {
      get { return _iDuration; }
      set
      {
        _iDuration = value;
        if (_iDuration < 0)
        {
          _iDuration = 0;
        }
      }
    }

    public int Year
    {
      get { return _iYear; }
      set
      {
        _iYear = value;
        if (_iYear < 0)
        {
          _iYear = 0;
        }
        else
        {
          if (_iYear > 0 && _iYear < 100)
          {
            _iYear += 1900;
          }
        }
      }
    }

    public int TimesPlayed { get; set; }

    public int Rating { get; set; }

    public bool Favorite { get; set; }

    public DateTime DateTimeModified { get; set; }

    /// <summary>
    /// Last UTC time the song was played
    /// </summary>
    public DateTime DateTimePlayed { get; set; }

    /// <summary>
    /// Determines whether the song has been submitted, is waiting for submit, etc
    /// </summary>
    public SongStatus AudioScrobblerStatus { get; set; }

    public string URL { get; set; }

    public string WebImage { get; set; }

    public string LastFMMatch { get; set; }

    public int ResumeAt
    {
      get { return _iResumeAt; }
      set
      {
        _iResumeAt = value;
        if (_iResumeAt < 0)
        {
          _iResumeAt = 0;
        }
      }
    }

    public int DiscId
    {
      get { return _iDisc; }
      set
      {
        _iDisc = value;
        if (_iDisc < 0)
        {
          _iDisc = 0;
        }
      }
    }

    public int DiscTotal
    {
      get { return _iNumDisc; }
      set
      {
        _iNumDisc = value;
        if (_iNumDisc < 0)
        {
          _iNumDisc = 0;
        }
      }
    }

    public string Lyrics { get; set; }

    public string Comment { get; set; }

    public string FileType { get; set; }

    public string Codec { get; set; }

    public string BitRateMode { get; set; }

    public int BPM { get; set; }

    public int BitRate { get; set; }

    public int Channels { get; set; }

    public int SampleRate { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzArtistId
    /// </summary>
    public string MusicBrainzArtistId { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzDisctId
    /// </summary>
    public string MusicBrainzDiscId { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzReleaseArtistId
    /// </summary>
    public string MusicBrainzReleaseArtistId { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzReleaseCountry
    /// </summary>
    public string MusicBrainzReleaseCountry { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzReleaseId
    /// </summary>
    public string MusicBrainzReleaseId { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzReleaseStatus
    /// </summary>
    public string MusicBrainzReleaseStatus { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzReleaseType
    /// </summary>
    public string MusicBrainzReleaseType { get; set; }

    /// <summary>
    /// Property to get/set the MusicBrainzReleaseTrackId
    /// </summary>
    public string MusicBrainzReleaseTrackId { get; set; }

    /// <summary>
    /// Property to get/set the MusicIpid
    /// </summary>
    public string MusicIpid { get; set; }

    /// <summary>
    /// Property to get/set the ReplayGain of the Track
    /// </summary>
    public string ReplayGainTrack { get; set; }

    /// <summary>
    /// Property to get/set the Peak of the Track
    /// </summary>
    public string ReplayGainTrackPeak { get; set; }

    /// <summary>
    /// Property to get/set the ReplayGain of the Album
    /// </summary>
    public string ReplayGainAlbum { get; set; }

    /// <summary>
    /// Property to get/set the Peak of the Album
    /// </summary>
    public string ReplayGainAlbumPeak { get; set; }

    /// <summary>
    /// Indicates whether the track has been choosen by the user / a radio station / a suggestion method
    /// </summary>
    public SongSource Source { get; set; }

    public string AuthToken { get; set; }

    /// <summary>
    /// A submit action like BAN / LOVE / SKIP
    /// </summary>
    public SongAction AudioscrobblerAction { get; set; }

    #endregion

    #region Methods

    public MusicTag ToMusicTag()
    {
      MusicTag tmpTag = new MusicTag();

      tmpTag.Title = Title;
      tmpTag.TitleSort = TitleSort;
      tmpTag.Album = Album;
      tmpTag.AlbumSort = AlbumSort;
      tmpTag.DiscID = DiscId;
      tmpTag.DiscTotal = DiscTotal;
      tmpTag.AlbumArtist = AlbumArtist;
      tmpTag.AlbumArtistSort = AlbumArtistSort;
      tmpTag.AmazonId = AmazonId;
      tmpTag.Artist = Artist;
      tmpTag.ArtistSort = ArtistSort;
      tmpTag.Duration = Duration;
      tmpTag.Genre = Genre;
      tmpTag.Grouping = Grouping;
      tmpTag.Composer = Composer;
      tmpTag.ComposerSort = ComposerSort;
      tmpTag.Conductor = Conductor;
      tmpTag.Copyright = Copyright;
      tmpTag.Track = Track;
      tmpTag.TrackTotal = TrackTotal;
      tmpTag.Year = Year;
      tmpTag.Rating = Rating;
      tmpTag.ReplayGainAlbum = ReplayGainAlbum;
      tmpTag.ReplayGainAlbumPeak = ReplayGainAlbumPeak;
      tmpTag.ReplayGainTrack = ReplayGainTrack;
      tmpTag.ReplayGainTrackPeak = ReplayGainTrackPeak;
      tmpTag.TimesPlayed = TimesPlayed;
      tmpTag.Lyrics = Lyrics;
      tmpTag.MusicBrainzArtistId = MusicBrainzArtistId;
      tmpTag.MusicBrainzDiscId = MusicBrainzDiscId;
      tmpTag.MusicBrainzReleaseArtistId = MusicBrainzReleaseArtistId;
      tmpTag.MusicBrainzReleaseCountry = MusicBrainzReleaseCountry;
      tmpTag.MusicBrainzReleaseId = MusicBrainzReleaseId;
      tmpTag.MusicBrainzReleaseStatus = MusicBrainzReleaseStatus;
      tmpTag.MusicBrainzReleaseTrackId = MusicBrainzReleaseTrackId;
      tmpTag.MusicBrainzReleaseType = MusicBrainzReleaseType;
      tmpTag.MusicIpid = MusicIpid;
      tmpTag.DateTimeModified = DateTimeModified;
      tmpTag.DateTimePlayed = DateTimePlayed;
      tmpTag.Comment = Comment;
      tmpTag.FileType = FileType;
      tmpTag.Codec = Codec;
      tmpTag.BitRateMode = BitRateMode;
      tmpTag.BPM = BPM;
      tmpTag.BitRate = BitRate;
      tmpTag.Channels = Channels;
      tmpTag.SampleRate = SampleRate;
      tmpTag.HasAlbumArtist = string.IsNullOrEmpty(AlbumArtist);

      return tmpTag;
    }

    public PlayListItem ToPlayListItem()
    {
      PlayListItem pli = new PlayListItem();

      pli.Type = PlayListItem.PlayListItemType.Audio;
      pli.FileName = this.FileName;
      pli.Description = this.Title;
      pli.Duration = this.Duration;
      MusicTag tag = this.ToMusicTag();
      pli.MusicTag = tag;

      return pli;
    }

    public string ToShortString()
    {
      StringBuilder s = new StringBuilder();

      if (Title != String.Empty)
      {
        s.Append(Title);
      }
      else
      {
        s.Append("(Untitled)");
      }
      if (_strArtist != String.Empty)
      {
        s.Append(" - " + _strArtist);
      }
      if (Album != String.Empty)
      {
        s.Append(" (" + Album + ")");
      }

      return s.ToString();
    }

    public string ToLastFMString()
    {
      StringBuilder s = new StringBuilder();

      if (_strArtist != String.Empty)
      {
        s.Append(_strArtist);
        s.Append(" - ");
      }
      if (Title != String.Empty)
      {
        s.Append(Title);
      }
      if (_iDuration > 0)
      {
        s.Append(" [");
        s.Append(Util.Utils.SecondsToHMSString(_iDuration));
        s.Append("]");
      }
      if (TimesPlayed > 0)
      {
        s.Append(" (played: " + Convert.ToString(TimesPlayed) + " times)");
      }

      return s.ToString();
    }

    public string ToLastFMMatchString(bool showURL_)
    {
      StringBuilder s = new StringBuilder();
      if (_strArtist != String.Empty)
      {
        s.Append(_strArtist);
        if (Album != String.Empty)
        {
          s.Append(" - " + Album);
        }
        else
        {
          if (Title != String.Empty)
          {
            s.Append(" - " + Title);
          }
          if (Genre != String.Empty)
          {
            s.Append(" (tagged: " + Genre + ")");
          }
        }
      }
      else if (Album != String.Empty)
      {
        s.Append(Album);
      }
      if (LastFMMatch != String.Empty)
      {
        if (LastFMMatch.IndexOf(".") == -1)
        {
          s.Append(" (match: " + LastFMMatch + "%)");
        }
        else
        {
          s.Append(" (match: " + LastFMMatch.Remove(LastFMMatch.IndexOf(".") + 2) + "%)");
        }
      }
      if (showURL_)
      {
        if (URL != String.Empty)
        {
          s.Append(" (link: " + URL + ")");
        }
      }

      return s.ToString();
    }

    public string ToURLArtistString()
    {
      return HttpUtility.UrlEncode(_strArtist);
    }

    public override string ToString()
    {
      return _strArtist + "\t" +
             Title + "\t" +
             Album + "\t" +
             _iDuration + "\t" +
             DateTimePlayed.ToString("s");
    }

    public string getQueueTime(bool asUnixTime)
    {
      string queueTime = string.Empty;

      if (asUnixTime)
      {
        queueTime = Convert.ToString(Util.Utils.GetUnixTime(DateTimePlayed.ToUniversalTime()));
      }
      else
      {
        queueTime = String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                                  DateTimePlayed.Year,
                                  DateTimePlayed.Month,
                                  DateTimePlayed.Day,
                                  DateTimePlayed.Hour,
                                  DateTimePlayed.Minute,
                                  DateTimePlayed.Second);
      }
      return queueTime;
    }

    public string getRateActionParam()
    {
      switch (AudioscrobblerAction)
      {
        case SongAction.N:
          return String.Empty;
        case SongAction.L:
          return "L";
        case SongAction.B:
          return "B";
        case SongAction.S:
          return "S";
        default:
          return String.Empty;
      }
    }

    public string getSourceParam()
    {
      switch (Source)
      {
        case SongSource.P:
          return "P";
        case SongSource.R:
          return "R";
        case SongSource.E:
          return "E";
        case SongSource.L:
          return "L" + AuthToken;
        case SongSource.U:
          return "U";
        default:
          return "P";
      }
    }

    #endregion
  }


  public class SongMap
  {
    public string m_strPath;
    public Song m_song;
  }
}