using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System.IO;
using System.Threading.Tasks;

namespace SharedLib.Services;

public class MinioService
{
    private readonly IMinioClient _minioClient;

    public MinioService(string endpoint, string accessKey, string secretKey)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }

    public async Task InitializeBucket(string bucketName)
    {
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(mbArgs);
        }
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
}