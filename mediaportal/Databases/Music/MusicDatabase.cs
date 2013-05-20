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
using System.Data;
using System.Globalization;
using System.IO;
using MediaPortal.Configuration;
using MediaPortal.Database;
using MediaPortal.Profile;
using MediaPortal.ServiceImplementations;
using SQLite.NET;
using System.Data.SQLite;

namespace MediaPortal.Music.Database
{
  /// <summary>
  /// Singleton class which establishes a database connection and provides lookup methods
  /// </summary>
  public partial class MusicDatabase
  {
    #region Variables

    public static readonly MusicDatabase Instance = new MusicDatabase();
    private const int DATABASE_VERSION = 1;

    private SQLiteConnection _dbConnection = null;
    private SQLiteClient MusicDbClient = null;

    private static bool _dbIsOpened = false;
    private static bool _treatFolderAsAlbum;
    private static bool _extractEmbededCoverArt;
    private static bool _useFolderThumbs;
    private static bool _useAllImages;
    private static bool _createArtistPreviews;
    private static bool _createGenrePreviews;
    private static bool _createMissingFolderThumbs;
    private static string _supportedExtensions;
    private static bool _stripArtistPrefixes;
    private static DateTime _lastImport;
    private static DateTime _currentDate = DateTime.Now;
    private static int _dateAddedValue;

    #endregion

    #region Constructors/Destructors

    /// <summary>
    /// static constructor. Opens or creates the music database
    /// </summary>
    static MusicDatabase() { }

    /// <summary>
    /// private constructor to prevent any instance of this class
    /// </summary>
    private MusicDatabase()
    {
      // Set default values 
      _treatFolderAsAlbum = false;
      _extractEmbededCoverArt = true;
      _useFolderThumbs = true;
      _useAllImages = true;
      _createArtistPreviews = false;
      _createGenrePreviews = true;
      _createMissingFolderThumbs = false;
      _supportedExtensions = Util.Utils.AudioExtensionsDefault;
      _stripArtistPrefixes = false;
      _currentDate = DateTime.Now;
      _dateAddedValue = 0;

      LoadDBSettings();
    }

    ~MusicDatabase() { }

    public static void ReOpen()
    {
      Dispose();
      Instance.Open();
    }

    public static void Dispose()
    {
      if (Instance._dbConnection != null)
      {
        Instance._dbConnection.Dispose();
      }
      Instance._dbConnection = null;

      if (Instance.MusicDbClient != null)
      {
        Instance.MusicDbClient.Dispose();
      }
      Instance.MusicDbClient = null;
    }

    #endregion

    #region Getters & Setters

    /// <summary>
    /// Gets the current DB Connection
    /// </summary>
    private SQLiteConnection DbConnection
    {
      get
      {
        if (Instance._dbConnection == null)
        {
          Instance._dbConnection = new SQLiteConnection(string.Format(@"Data Source={0}", Config.GetFile(Config.Dir.Database, "MusicDatabase.db3")));
        }

        if (!_dbIsOpened)
        {
          Open();
        }

        if (Instance._dbConnection.State != ConnectionState.Open)
        {
          Instance._dbConnection.Open();
        }

        return Instance._dbConnection;
      }
    }

    /// <summary>
    /// Gets the current Database Name
    /// </summary>
    public string DatabaseName
    {
      get { return DbConnection.Database; }
    }

    #endregion

    #region Functions

    /// <summary>
    /// Execute the provide SQL statement against the current database
    /// </summary>
    /// <param name="aSQL"></param>
    /// <returns>Result set of rows</returns>
    public static SQLiteResultSet DirectExecute(string aSQL)
    {
      SQLiteResultSet result = new SQLiteResultSet();
      return result;
    }

    /// <summary>
    /// Execute a Query Statement against the current database
    /// </summary>
    /// <param name="aSQL"></param>
    /// <returns></returns>
    public static SQLiteResultSet ExecuteQuery(string aSQL)
    {
      var resultSet = new SQLiteResultSet();

      try
      {
        using (var cmd = new SQLiteCommand())
        {
          cmd.Connection = Instance.DbConnection;
          cmd.CommandText = aSQL;
          using (SQLiteDataReader reader = cmd.ExecuteReader())
          {
            if (reader.HasRows)
            {
              resultSet.LastCommand = aSQL;
              for (var i = 0; i < reader.FieldCount; i++)
              {
                var columnName = reader.GetName(i);
                resultSet.columnNames.Add(columnName);
                resultSet.ColumnIndices[columnName] = i;
              }
            }

            while (reader.Read())
            {
              var row = new SQLiteResultSet.Row();
              for (var i = 0; i < reader.FieldCount; i++)
              {
                var columnValue = (string)reader.GetValue(i);
                row.fields.Add(columnValue);
              }
              resultSet.Rows.Add(row);
            }
          }
          return resultSet;
        }
      }
      catch (Exception ex)
      {
        Log.Error("MusicDatabase: Exception executing: {0}\\n{1}", aSQL, ex.Message);
        return resultSet;
      }
    }

    /// <summary>
    /// Execute a Non-Query Statement against the current database
    /// </summary>
    /// <param name="aSQL"></param>
    /// <returns></returns>
    public static bool ExecuteNonQuery(string aSQL)
    {
      try
      {
        using (SQLiteCommand cmd = new SQLiteCommand())
        {
          cmd.Connection = Instance.DbConnection;
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = aSQL;
          cmd.ExecuteNonQuery();
          return true;
        }
      }
      catch (Exception ex)
      {
        Log.Error("MusicDatabase: Exception executing: {0}/n{1}", aSQL, ex.Message);
        return false;
      }
    }

    /// <summary>
    /// Load the settings from the config files
    /// </summary>
    private void LoadDBSettings()
    {
      using (Settings xmlreader = new MPSettings())
      {
        _treatFolderAsAlbum = xmlreader.GetValueAsBool("musicfiles", "treatFolderAsAlbum", false);
        _extractEmbededCoverArt = xmlreader.GetValueAsBool("musicfiles", "extractthumbs", true);
        _useFolderThumbs = xmlreader.GetValueAsBool("musicfiles", "useFolderThumbs", true);
        _useAllImages = xmlreader.GetValueAsBool("musicfiles", "useAllImages", _useFolderThumbs);
        _createMissingFolderThumbs = xmlreader.GetValueAsBool("musicfiles", "createMissingFolderThumbs",
                                                              _treatFolderAsAlbum);
        _createArtistPreviews = xmlreader.GetValueAsBool("musicfiles", "createartistthumbs", false);
        _createGenrePreviews = xmlreader.GetValueAsBool("musicfiles", "creategenrethumbs", true);
        _supportedExtensions = xmlreader.GetValueAsString("music", "extensions", Util.Utils.AudioExtensionsDefault);
        _stripArtistPrefixes = xmlreader.GetValueAsBool("musicfiles", "stripartistprefixes", false);
        _dateAddedValue = xmlreader.GetValueAsInt("musicfiles", "dateadded", 0);
        _updateSinceLastImport = xmlreader.GetValueAsBool("musicfiles", "updateSinceLastImport", false);

        try
        {
          string lastImport = xmlreader.GetValueAsString("musicfiles", "lastImport", "1900-01-01 00:00:00");
          _lastImport = DateTime.ParseExact(lastImport, "yyyy-M-d H:m:s", CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
          _lastImport = DateTime.ParseExact("1900-01-01 00:00:00", "yyyy-M-d H:m:s", CultureInfo.InvariantCulture);
        }
      }
    }

    /// <summary>
    /// Open the Music Database. If it doesn't exist create it.
    /// </summary>
    private void Open()
    {
      Log.Info("MusicDatabase: Opening database");
      _dbIsOpened = true;

      try
      {
        // Open database
        try
        {
          Directory.CreateDirectory(Config.GetFolder(Config.Dir.Database));
        }
        catch (Exception) { }

        if (!File.Exists(Config.GetFile(Config.Dir.Database, "MusicDatabase.db3")))
        {
          if (File.Exists(Config.GetFile(Config.Dir.Database, "MusicDatabaseV12.db3")))
          {
            Log.Info("MusicDatabase: Found older, incompatible version of database. Backup old database and create new layout.");
            File.Move(Config.GetFile(Config.Dir.Database, "MusicDatabaseV12.db3"),
                      Config.GetFile(Config.Dir.Database, "MusicDatabaseV12-backup.db3"));
          }

          // Get the DB handle or create it if necessary
          _dbConnection = DbConnection;

          // When we have deleted the database, we need to scan from the beginning, regardsless of the last import setting
          _lastImport = DateTime.ParseExact("1900-01-01 00:00:00", "yyyy-M-d H:m:s", CultureInfo.InvariantCulture);

          Log.Info("MusicDatabase: Database does not exist. Create it.");
          if (!CreateDatabase())
          {
            return;
          }
        }

        _dbConnection = DbConnection;

        // See, if we're running the correct Version
        strSQL = "select Value from Configuration where Parameter = 'Version'";
        SQLiteResultSet results = ExecuteQuery(strSQL);
        if (Int32.Parse(results.Rows[0].fields[0]) != DATABASE_VERSION)
        {
          // Do upgrade logic to new version here
          // Currently not necessary, since we are starting at Version 1.
        }
      }

      catch (Exception ex)
      {
        Log.Error("MusicDatabase: exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
      }
      Log.Info("MusicDatabase: Database opened");
    }

    private bool CreateDatabase()
    {
      try
      {
        ExecuteNonQuery("PRAGMA encoding = \"UTF-8\"");
        ExecuteNonQuery("PRAGMA cache_size=4096");
        ExecuteNonQuery("PRAGMA page_size=8192");
        ExecuteNonQuery("PRAGMA synchronous='OFF'");
        ExecuteNonQuery("PRAGMA auto_vacuum=0");
        ExecuteNonQuery("PRAGMA foreign_keys='ON'");

        // The Configuration Table holds various information about the database
        ExecuteNonQuery(@"create table Configuration (Parameter string not null, Value string not null)");

        // Insert the Database Version number into the Config Table
        ExecuteNonQuery(string.Format("insert into Configuration values ('Version', '{0}')", DATABASE_VERSION));

        // Share Table: Holds information about the Music Shares
        ExecuteNonQuery("CREATE TABLE Share (" +
                                 "Id integer primary key," +
                                 "ShareName text not null" +
        ")");

        // Folder Table: Holds information about the different Folder
        ExecuteNonQuery("CREATE TABLE Folder (" +
                                 "Id integer primary key," +
                                 "IdShare integer not null," +
                                 "FolderName text not null," +
                                 "Foreign Key(IdShare) References Share(Id)" +
        ")");

        // Artist Table
        // Holds Information about Artist, AlbumArtist, Composer, Conductor
        ExecuteNonQuery("CREATE TABLE Artist (" +
                                 "Id integer primary key," +
                                 "ArtistName text not null," +
                                 "ArtistSortName text not null" +
        ")");

        ExecuteNonQuery("CREATE INDEX IdxArtist_ArtistName ON Artist(ArtistName ASC)");
        
        // Album Table
        ExecuteNonQuery("CREATE TABLE Album (" +
                                 "Id integer primary key," +
                                 "AlbumName text not null," +
                                 "AlbumSortName text not null," +
                                 "Year integer" +
        ")");

        ExecuteNonQuery("CREATE INDEX IdxAlbum_AlbumName ON Album(AlbumName ASC)");

        // Genre Table
        ExecuteNonQuery("CREATE TABLE Genre (" +
                                 "Id integer primary key," +
                                 "GenreName text not null" +
        ")");

        ExecuteNonQuery("CREATE INDEX IdxGenre_GenreName ON Genre(GenreName ASC)");

        // Song table containing information for songs
        ExecuteNonQuery(
          @"CREATE TABLE Song ( " +
          "Id integer primary key, " + // Unique Song id. Manually incremented
          "IdFolder integer not null, " + // ID of the folder. Foreign Key
          "IdAlbum integer not null, " + // ID of the album. Foreign Key
          "FileName text not null, " + // FileName of song. Needs to be concatenated with ShareNAme + FolderName for Fullpath
          "Title text not null, " + // Song Title
          "TitleSort text, " + // Song Title for Sorting
          "Track integer, " + // Track Number
          "TrackCount integer, " + // Total  Number of Tracks on Album
          "Disc integer, " + // Disc Number
          "DiscCount integer, " + // Total  Number of Discs
          "Duration integer, " + // Duration in seconds
          "Year integer, " + // Year
          "TimesPlayed integer, " + // # Times Played
          "Rating integer, " + // Rating
          "Favorite integer, " + // Favorite Indicator
          "ResumeAt integer, " + // Resume  song from position
          "Lyrics text, " + // Lyric Text
          "Comment text, " + // Comment
          "Copyright text, " + // Copyright Information
          "AmazonId text, " + // The AmazonId
          "Grouping text, " + // Grouping Information
          "MusicBrainzArtistId text, " + // MusicBrainz ArtistId
          "MusicBrainzDiscId text, " + // MusicBrainz DiscId
          "MusicBrainzReleaseArtistId text, " + // MusicBrainz Release ArtistId
          "MusicBrainzReleaseCountry text, " + // MusicBrainz Release Country
          "MusicBrainzReleaseId text, " + // MusicBrainz ReleaseId
          "MusicBrainzReleaseStatus text, " + // MusicBrainz Release Status
          "MusicBrainzReleaseTrackId text, " + // MusicBrainz Release TrackId
          "MusicBrainzReleaseType text, " + // MusicBrainzReleaseType
          "MusicIpid text, " + // ID from MusicIp
          "ReplayGainTrack text, " + // Track ReplayGain
          "ReplayGainTrackPeak text, " + // Peak for Track
          "ReplayGainAlbum text, " + // Album ReplayGain
          "ReplayGainAlbumPeak text, " + // Peak for Album
          "FileType text, " + // File Format (mp3, flac, etc.)           
          "Codec text, " + // Full Codec Description      
          "BitRateMode text, " + // Bitrate mode (CBR / VBR)           
          "BPM integer, " + // Beats per Minute
          "BitRate integer, " + // Bitrate
          "Channels integer, " + // Channels
          "SampleRate integer, " + // Sample Rate    
          "DateLastPlayed timestamp, " + // Date, Last Time Played
          "DateAdded timestamp, " + // Date added. Either Insertion date, Creation date, LastWrite
          "Foreign Key(IdFolder) references Folder(Id), " +
          "Foreign Key(IdAlbum) references Album(Id)" +
          ")"
          );

        ExecuteNonQuery("CREATE INDEX IdxSong_FileName ON Song(FileName ASC)");
        
        // AlbumArtist: Relation between Artist - Album  
        ExecuteNonQuery("CREATE TABLE AlbumArtist (" +
                                 "IdArtist integer not null," + 
                                 "IdAlbum integer not null," + 
                                 "Primary Key(IdArtist, IdAlbum)" +
                                 ")");

        // ArtistSong: Relation between Artist - Song  
        ExecuteNonQuery("CREATE TABLE ArtistSong (" +
                                 "IdArtist integer not null," +
                                 "IdSong integer not null," +
                                 "Primary Key(IdArtist, IdSong)" +
                                 ")");

        // GenreSong: Relation between Genre - Song  
        ExecuteNonQuery("CREATE TABLE GenreSong (" +
                                 "IdGenre integer not null," +
                                 "IdSong integer not null," +
                                 "Primary Key(IdGenre, IdSong)" +
                                 ")");

        // ComposerSong: Relation between Composer - Song  
        ExecuteNonQuery("CREATE TABLE ComposerSong (" +
                                 "IdComposer integer not null," +
                                 "IdSong integer not null," +
                                 "Primary Key(IdComposer, IdSong)" +
                                 ")");

        // ConductorSong: Relation between Conductor - Song  
        ExecuteNonQuery("CREATE TABLE ConductorSong (" +
                                 "IdConductor integer not null," +
                                 "IdSong integer not null," +
                                 "Primary Key(IdConductor, IdSong)" +
                                 ")");


        // Artist Info and Album Info
        ExecuteNonQuery("CREATE TABLE albuminfo ( idAlbumInfo integer primary key autoincrement, strAlbum text, strArtist text, strAlbumArtist text,iYear integer, idGenre integer, strTones text, strStyles text, strReview text, strImage text, strTracks text, iRating integer)");
        ExecuteNonQuery("CREATE TABLE artistinfo ( idArtistInfo integer primary key autoincrement, strArtist text, strBorn text, strYearsActive text, strGenres text, strTones text, strStyles text, strInstruments text, strImage text, strAMGBio text, strAlbums text, strCompilations text, strSingles text, strMisc text)");

        // Indices for Album and Artist Info
        ExecuteNonQuery("CREATE INDEX idxalbuminfo_strAlbum ON albuminfo(strAlbum ASC)");
        ExecuteNonQuery("CREATE INDEX idxalbuminfo_strArtist ON albuminfo(strArtist ASC)");
        ExecuteNonQuery("CREATE INDEX idxalbuminfo_idGenre ON albuminfo(idGenre ASC)");
        ExecuteNonQuery("CREATE INDEX idxartistinfo_strArtist ON artistinfo(strArtist ASC)");

        // Scrobble table
        ExecuteNonQuery("CREATE TABLE scrobbleusers ( idScrobbleUser integer primary key, strUsername text, strPassword text)");
        ExecuteNonQuery("CREATE TABLE scrobblesettings ( idScrobbleSettings integer primary key, idScrobbleUser integer, iAddArtists integer, iAddTracks integer, iNeighbourMode integer, iRandomness integer, iScrobbleDefault integer, iSubmitOn integer, iDebugLog integer, iOfflineMode integer, iPlaylistLimit integer, iPreferCount integer, iRememberStartArtist integer, iAnnounce integer)");
        ExecuteNonQuery("CREATE TABLE scrobblemode ( idScrobbleMode integer primary key, idScrobbleUser integer, iSortID integer, strModeName text)");
        ExecuteNonQuery("CREATE TABLE scrobbletags ( idScrobbleTag integer primary key, idScrobbleMode integer, iSortID integer, strTagName text)");

        Log.Info("MusicDatabase: New Database created successfully");
        return true;
      }
      catch (Exception ex)
      {
        Log.Error("MusicDatabase: Create of database failed. Err:{0} stack:{1}", ex.Message, ex.StackTrace);
      }
      return false;
    }

    #endregion

    #region Transactions

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    public void BeginTransaction()
    {
      try
      {
        ExecuteNonQuery("begin");
      }
      catch (Exception ex)
      {
        Log.Error("MusicDatabase: BeginTransaction: musicdatabase begin transaction failed exception err:{0} ",
                  ex.Message);
      }
    }

    /// <summary>
    /// End a database transaction, commiting all changed rows
    /// </summary>
    public void CommitTransaction()
    {
      Log.Debug("MusicDatabase: Commit will effect {0} rows", _dbConnection.Changes);
      try
      {
        ExecuteNonQuery("commit");
      }
      catch (Exception ex)
      {
        Log.Error("MusicDatabase: Commit failed exception err:{0} ", ex.Message);
        Open();
      }
    }

    /// <summary>
    /// Rollback a database transactio, reverting all changes
    /// </summary>
    public void RollbackTransaction()
    {
      Log.Debug("MusicDatabase: Rolling back transactions due to unrecoverable error. Effecting {0} rows",
                _dbConnection.Changes);
      try
      {
        ExecuteNonQuery("rollback");
      }
      catch (Exception ex)
      {
        Log.Error("MusicDatabase: Rollback failed exception err:{0} ", ex.Message);
        Open();
      }
    }

    #endregion
  }
}