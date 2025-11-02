using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;


namespace SharedLib.Services;

public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _bucketName = "blog-photos";

    public MinioService(ILogger<MinioService> logger, IHttpClientFactory httpClientFactory, string endpoint, string accessKey, string secretKey)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }

    public async Task<string> UploadPhoto(string objectName, Stream fileStream)
    {
        var beArgs = new BucketExistsArgs().WithBucket(_bucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(mbArgs);
            _logger.LogInformation($"Bucket created: {_bucketName}");
        }
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length);

        await _minioClient.PutObjectAsync(putObjectArgs);
        _logger.LogInformation($"Uploaded photo: {objectName} to {_bucketName}");
        
        return _bucketName;
    }
    
    public async Task<string?> GetPhotoUrlAsync(Photo photo, int expiryInSeconds = 3600)
    {
        if (!PhotoExists(photo))
        {
            _logger.LogInformation($"Photo don't have: Bucketname: {photo.BucketName}, or Objectname: {photo.ObjectName}");
            return null;
        }
        
        var args = new PresignedGetObjectArgs()
            .WithBucket(photo.BucketName)
            .WithObject(photo.ObjectName)
            .WithExpiry(expiryInSeconds);

        var url = await _minioClient.PresignedGetObjectAsync(args);
        _logger.LogInformation($"Returned URL for: {photo.BucketName}/{photo.ObjectName}");
        
        return url;
    }
    
    public async Task DeletePhoto(Photo photo)
    {
        if (!PhotoExists(photo))
        {
            _logger.LogInformation($"Photo don't have: Bucketname: {photo.BucketName}, or Objectname: {photo.ObjectName}");
            return;
        }
        
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(photo.BucketName)
            .WithObject(photo.ObjectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs);
        _logger.LogInformation($"Deleting {photo.ObjectName}, from {photo.BucketName}.");
    }

    bool PhotoExists(Photo photo)
    {
        if (string.IsNullOrEmpty(photo.BucketName) || string.IsNullOrEmpty(photo.ObjectName))
            return false;
        return true;
    }
    
    public async Task<byte[]> DownloadImageAsync(Photo photo)
    {
        // Get url
        var imageUrl = await GetPhotoUrlAsync(photo);
        if (string.IsNullOrEmpty(imageUrl))
            return null;
        _logger.LogInformation($"Downloading url: {imageUrl}");
        
        // Download bytes
        byte[] imageBytes;
        try
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation($"Downloaded image status code: {response.StatusCode}");
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to download image from {ImageUrl}", imageUrl);
            throw;
        }
        //
        // // Convert to string
        // if (imageBytes != null && imageBytes.Length > 0)
        //     return null;
        //
        // string base64Image = Convert.ToBase64String(imageBytes);
        // string imageSrc = $"data:image/jpeg;base64,{base64Image}";
        // return imageSrc;
    
    }
}