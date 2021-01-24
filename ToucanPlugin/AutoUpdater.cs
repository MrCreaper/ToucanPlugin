using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Updater.GHApi.Models;
using UnityEngine;

namespace ToucanPlugin
{
    public static class AutoUpdater
    {
        public const long REPOID = 297982389;
        public static string GitHubGetReleasesTemplate = $"https://api.github.com/repositories/{REPOID}/releases";
        private static HttpClient httpClient = new HttpClient();
        public static void CheckReleases()
        {
            var url = string.Format(GitHubGetReleasesTemplate, REPOID);

            using (var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(1000)))
            using (var result = httpClient.GetAsync(url, cancellationToken.Token).ConfigureAwait(false).GetAwaiter().GetResult())
            using (var streamResult = result.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
            {
                var releases = Utf8Json.JsonSerializer.Deserialize<Release[]>(streamResult)
                    .Where(r => Version.TryParse(r.TagName, out _))
                    .OrderByDescending(r => r.CreatedAt.Ticks)
                    .ToArray();

                Log.Error(releases);
            }
        }

        public static async Task RunUpdater(int waitTime = 0)
        {
            if (waitTime != 0) await Task.Delay(waitTime);

            using (var client = new WebClient())
            {
                //var res = await client.DownloadStringTaskAsync("https://scpsl.mrtoucan.dev/update/version");
                //if (res == Version) return;

                var location = Directory.GetFiles(Paths.Plugins).FirstOrDefault(path => path.ToLower().Contains(ToucanPlugin.Instance.Name.ToLower()) && path.EndsWith(".dll"));
                if (location == null)
                {
                    Log.Warn("Why did you rename the plugin?... why?");
                    return;
                }

                await client.DownloadFileTaskAsync($"https://github.com/MrCreaper/ToucanPlugin/releases/latest/download/ToucanPlugin.dll", location);
                Log.Info($"Updated {ToucanPlugin.Instance.Name} {ToucanPlugin.Instance.Version}=>{"New Version here"}. Restarting server...");
                Application.Quit();
            }
        }
    }
}