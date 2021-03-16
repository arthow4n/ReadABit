using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace ReadABit.CliUtils.Commands
{
    public static class CertCommandHandler
    {
        public static void Handle(IHost host)
        {
            Console.WriteLine($"dotnet user-secrets set \"Certificates:OpenIddictEncryption\" \"{CreateCert("ReadABit OpenIddict Server Encryption Certificate", X509KeyUsageFlags.KeyEncipherment)}\"");
            Console.WriteLine($"dotnet user-secrets set \"Certificates:OpenIddictSigning\" \"{CreateCert("ReadABit OpenIddict Server Signing Certificate", X509KeyUsageFlags.DigitalSignature)}\"");
            Console.WriteLine($"\n\nRun the commands above in ReadABit.Web to setup the certificates!");
        }

        private static string CreateCert(string cn, X509KeyUsageFlags usage)
        {
            using var algo = RSA.Create(keySizeInBits: 4096);
            var request = new CertificateRequest(
                new X500DistinguishedName($"CN={cn}"),
                algo,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(usage, critical: true)
            );

            var cert = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(4));
            var data = cert.Export(X509ContentType.Pfx, string.Empty);

            return Convert.ToBase64String(data);
        }
    }
}
