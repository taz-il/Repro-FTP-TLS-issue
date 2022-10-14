using System;
using System.IO;
using System.Net;

namespace ReproFTP_FtpWebRequest
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 6)
            {
                Console.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]} [host] [username] [password] [plaintext|tls12|tls13] [local] [remote]");
                return 1;
            }

            var host = args[0];
            var username = args[1];
            var password = args[2];

            var encryptionArg = args[3];
            bool enableSsl = false;
            if (encryptionArg == "tls12")
            {
                enableSsl = true;  
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            else if (encryptionArg == "tls13")
            {
                enableSsl = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            }
            Console.WriteLine(ServicePointManager.SecurityProtocol);

            var local = args[4];
            if (!File.Exists(local))
            {
                throw new InvalidOperationException($"File {local} does not exist");
            }

            var remote = args[5];

            var remoteUri = new UriBuilder() { Scheme = "ftp", Host = host, Path = remote };
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteUri.Uri);
            request.Credentials = new NetworkCredential(username, password);
            request.EnableSsl = enableSsl;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            using (Stream fileStream = File.OpenRead(local))
            using (Stream ftpStream = request.GetRequestStream())
            {
                fileStream.CopyTo(ftpStream);
            }

            Console.WriteLine(">>> Success !! <<<");

            return 0;
        }
    }
}
