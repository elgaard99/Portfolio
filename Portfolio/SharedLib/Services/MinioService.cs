using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;


namespace SharedLib.Services;

public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioService> _logger;

    public MinioService(ILogger<MinioService> logger, string endpoint, string accessKey, string secretKey)
    {
        _logger = logger;
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }

    public async Task UploadPhoto(string bucketName, Stream fileStream, string fileName)
    {
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(mbArgs);
        }
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length);

        await _minioClient.PutObjectAsync(putObjectArgs);
    }
    
    public async Task<string> GetPhotoUrlAsync(string bucketName, string fileName, int expiryInSeconds = 3600)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithExpiry(expiryInSeconds);

        return await _minioClient.PresignedGetObjectAsync(args);
    }
    
    public async Task DeletePhoto(string bucketName, string objectName)
    {
        _logger.LogInformation($"Deleting {objectName}, from {bucketName}.");
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs);
    }
}