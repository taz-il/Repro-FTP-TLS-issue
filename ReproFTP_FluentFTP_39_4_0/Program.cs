using FluentFTP;
using System;
using System.IO;

namespace ReproFTP_FluentFTP_39_4_0
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
            var encryptionMode = FtpEncryptionMode.None;
            var sslProtocols = System.Security.Authentication.SslProtocols.None;
            if (encryptionArg == "tls12")
            {
                encryptionMode = FtpEncryptionMode.Explicit;
                sslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            } else if (encryptionArg == "tls13")
            {
                encryptionMode = FtpEncryptionMode.Explicit;
                sslProtocols = System.Security.Authentication.SslProtocols.Tls13;
            }
            Console.WriteLine(sslProtocols);

            var local = args[4];
            if (!File.Exists(local))
            {
                throw new InvalidOperationException($"File {local} does not exist");
            }

            var remote = args[5];

            FluentFTP.Helpers.FtpTrace.EnableTracing = true;
            FluentFTP.Helpers.FtpTrace.LogFunctions = true;

            using (var client = new FtpClient(host, username, password))
            {
                client.OnLogEvent = (x, m) => Console.WriteLine($"[{x}] {m}");
                client.EncryptionMode = encryptionMode;
                client.SslProtocols = sslProtocols;
                client.ValidateAnyCertificate = true;
                client.Connect();

                if (client.UploadFile(local, remote) == FtpStatus.Success)
                {
                    Console.WriteLine(">>> Success!! <<<");
                } else
                {
                    Console.WriteLine(">>> Error! <<<");
                }
            }

            return 0;
        }
    }
}
