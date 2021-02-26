using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ReadABit.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NativeLibrary.SetDllImportResolver(typeof(Program).Assembly, ImportResolver);
            CreateHostBuilder(args).Build().Run();
        }

        public static string UDPipeRuntimeDirPath =
            Path.Join(
                Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location),
                "/Integrations/Ufal/UDPipe/runtime/"
            );

        private readonly static string UDPipeDllImportName = "udpipe_csharp";

        // https://stackoverflow.com/a/60224214
        private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr libHandle = IntPtr.Zero;
            if (libraryName == UDPipeDllImportName && RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.Is64BitOperatingSystem)
            {
                NativeLibrary.TryLoad(Path.Join(UDPipeRuntimeDirPath, $"{UDPipeDllImportName}.dll"), out libHandle);
            }
            else if (libraryName == UDPipeDllImportName)
            {
                NativeLibrary.TryLoad($"lib{UDPipeDllImportName}.so", assembly, DllImportSearchPath.ApplicationDirectory, out libHandle);
            }
            return libHandle;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
