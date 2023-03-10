using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.IO;
using Google.Cloud.Storage.V1;
using System.Security.AccessControl;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        // SFTP server settings
        string host = "localhost";
        int port = 22;
        string username = "bill";
        string password = "example";
        string remoteFilePath = "/Users/bill/Desktop/remote/test.zip";
        string localFilePath = @"C:\Users\bill\Desktop\local\file.zip";
        string unzipFolderPath = @"C:\Users\bill\Desktop\local";
        string remoteDirectoryPath = "/";

        // Google Cloud Storage settings
        string bucketName = "your-unique-bucket-name";
        string localPath = "my-local-path/my-file-name";
        string objectName = localFilePath ;


        try
        {
            ListDirectoryContents(host, port, username, password, remoteDirectoryPath); // check root path
            DownloadFileFromSftp(host, port, username, password, remoteFilePath, localFilePath, unzipFolderPath);
            Console.WriteLine("File downloaded successfully.");
            /*var storage = StorageClient.Create();
            using var fileStream = File.OpenRead(localPath);
            storage.UploadObject(bucketName, objectName, null, fileStream);
            Console.WriteLine($"Uploaded {objectName}.");*/
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }


    static void DownloadFileFromSftp(string host, int port, string username, string password, string remoteFilePath, string localFilePath, string unzipFolderPath)
    {
        using (var client = new SftpClient(host, port, username, password))
        {
            client.Connect();
            using (var fileStream = new FileStream(localFilePath, FileMode.Create))
            {
                client.DownloadFile(remoteFilePath, fileStream);
            }
            client.Disconnect();
            ZipFile.ExtractToDirectory(localFilePath, unzipFolderPath);
        }
    }

    static void ListDirectoryContents(string host, int port, string username, string password, string remoteDirectoryPath)
    {
        using (var client = new SftpClient(host, port, username, password))
        {
            client.Connect();
            client.ChangeDirectory(remoteDirectoryPath);
            var directoryContents = client.ListDirectory(remoteDirectoryPath);
            foreach (var item in directoryContents)
            {
                Console.WriteLine(item.Name);
            }
            client.Disconnect();
        }
    }
}
