using System.Data;
using GalloTube.Interfaces;
using GalloTube.Models;
using MySql.Data.MySqlClient;

namespace GalloTube.Repositories;

public class VideoRepository : IVideoRepository
{
    readonly string connectionString = "server=localhost;port=3306;database=GalloTubedb;uid=root;pwd=''";
    readonly IVideoTagRepository _videoTagRepository;

public VideoRepository(IVideoTagRepository videoTagRepository)
    {
        _videoTagRepository = videoTagRepository;
    }


    public void Create(Video model)
    {
        MySqlConnection connection = new(connectionString);
        string sql = "insert into Video(Name, Description, UploadDate, Duration, Thumbnail, VideoFile) "
              + "values (@Name, @Description, @UploadDate, @Duration, @Thumbnail, @VideoFile)";
        MySqlCommand command = new(sql, connection)
        {
            CommandType = CommandType.Text
        };
        command.Parameters.AddWithValue("@Name", model.Name);
        command.Parameters.AddWithValue("@Description", model.Description);
        command.Parameters.AddWithValue("@UploadDate", model.UploadDate);
        command.Parameters.AddWithValue("@Duration", model.Duration);
        command.Parameters.AddWithValue("@Thumbnail", model.Thumbnail);
        command.Parameters.AddWithValue("@VideoFile", model.VideoFile);
        
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void Delete(int? id)
    {
        MySqlConnection connection = new(connectionString);
        string sql = "delete from Video where Id = @Id";
        MySqlCommand command = new(sql, connection)
        {
            CommandType = CommandType.Text
        };
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public List<Video> ReadAll()
    {
        MySqlConnection connection = new(connectionString);
        string sql = "select * from Video";
        MySqlCommand command = new(sql, connection)
        {
            CommandType = CommandType.Text
        };

        List<Video> Videos = new();
        connection.Open();
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Video Video = new()
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description"),
                UploadDate = reader.GetDateTime("uploadDate"),
                Duration = reader.GetInt16("duration"),
                Thumbnail = reader.GetString("thumbnail"),
                VideoFile = reader.GetString("videoFile")
            };
            Videos.Add(Video);
        }
        connection.Close();
        return Videos;
    }

    public Video ReadById(int? id)
    {
        MySqlConnection connection = new(connectionString);
        string sql = "select * from Video where Id = @Id";
        MySqlCommand command = new(sql, connection)
        {
            CommandType = CommandType.Text
        };
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        MySqlDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.HasRows)
        {
            Video Video = new()
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description"),
                UploadDate = reader.GetDateTime("uploadDate"),
                Duration = reader.GetInt16("duration"),
                Thumbnail = reader.GetString("thumbnail"),
                VideoFile = reader.GetString("videoFile")
            };
            connection.Close();
            return Video;
        }
        connection.Close();
        return null;
    }

    public void Update(Video model)
    {
        MySqlConnection connection = new(connectionString);
        string sql = "update Video set "
                        + "Name = @Name, "
                        + "Description = @Description, "
                        + "UploadDate = @UploadDate, "
                        + "Duration = @Duration, "
                        + "Thumbnail = @Thumbnail, "
                        + "VideoFile = @VideoFile "
                    + "where Id = @Id";
        MySqlCommand command = new(sql, connection)
        {
            CommandType = CommandType.Text
        };
        command.Parameters.AddWithValue("@Name", model.Name);
        command.Parameters.AddWithValue("@Description", model.Description);
        command.Parameters.AddWithValue("@UploadDate", model.UploadDate);
        command.Parameters.AddWithValue("@Duration", model.Duration);
        command.Parameters.AddWithValue("@Thumbnail", model.Thumbnail);
        command.Parameters.AddWithValue("@VideoFile", model.VideoFile);
        
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public List<Video> ReadAllDetailed()
    {
        List<Video> Videos = ReadAll();
        foreach (Video video in Videos)
        {
            video.Tags = _videoTagRepository.ReadTagsByVideo(video.Id);
        }
        return Videos;
    }

    public Video ReadByIdDetailed(int id)
    {
        Video video = ReadById(id);
        video.Tags = _videoTagRepository.ReadTagsByVideo(video.Id);
        return video;
    }
}