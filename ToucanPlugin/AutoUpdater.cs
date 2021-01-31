using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using Utf8Json;

namespace ToucanPlugin
{
    public class AutoUpdater
    {
        public const long REPOID = 297982389;
        public static string GitHubGetReleasesTemplate = $"https://api.github.com/repositories/{REPOID}/releases";
        private static HttpClient httpClient = CreateHttpClient();

        public static async Task<bool> UpToDate()
        {
            Release[] Releases = await GetReleases(REPOID);
            if (ToucanPlugin.Instance.VersionStr == Releases[0].TagName) return true;
            return false;
        }

        public static async Task Update()
        {
            using (var client = new WebClient())
            {
                Release[] Releases = await GetReleases(REPOID); // I mean you can save this but maybe later?
                //var res = await client.DownloadStringTaskAsync("https://scpsl.mrtoucan.dev/update/version");
                //if (res == Version) return;

                var location = Directory.GetFiles(Paths.Plugins).FirstOrDefault(path => path.ToLower().Contains(ToucanPlugin.Instance.Name.ToLower()) && path.EndsWith(".dll"));
                /*if (location == null)
                {
                    Log.Warn("Why did you rename the plugin?... why?");
                    return;
                }*/
                await client.DownloadFileTaskAsync($"https://github.com/MrCreaper/ToucanPlugin/releases/latest/download/ToucanPlugin.dll", location);
                Log.Info($"Updated {ToucanPlugin.Instance.Name} {ToucanPlugin.Instance.Version}=>{Releases[0].TagName}. Restarting server...");
                Application.Quit();
            }
        }

        public static async Task<Release[]> GetReleases(long repoId)
        {
            Release[] array;
            using (HttpResponseMessage httpResponse = await httpClient.GetAsync(string.Format("https://api.github.com/repositories/{0}/releases", (object)repoId)).ConfigureAwait(false))
            {
                using (Stream stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    array = ((IEnumerable<Release>)JsonSerializer.Deserialize<Release[]>(stream)).OrderByDescending(r => r.CreatedAt.Ticks).ToArray<Release>();
                }
            }
            return array;
        }
        // Warning! Exiled wizardry beyond this point!
        // ( really like what did Exiled do?! )
        private static HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(480.0),
                DefaultRequestHeaders =
                {
                    {
                        "User-Agent",
                        "FUCK (https://github.com/MrCreaper/ToucanPlugin, " + ToucanPlugin.Instance.Assembly.GetName().Version.ToString(3) + ")"
                    }
                }
            };
        }
    }
    public readonly struct Release : IJsonSerializable
    {
        public Release(int id, string tag_name, bool prerelease, DateTime created_at, ReleaseAsset[] assets)
        {
            Id = id;
            TagName = tag_name;
            PreRelease = prerelease;
            CreatedAt = created_at;
            Assets = assets;
        }

        public readonly int Id;

        public readonly string TagName;

        public readonly bool PreRelease;

        public readonly DateTime CreatedAt;

        public readonly ReleaseAsset[] Assets;
    }
    public readonly struct ReleaseAsset
    {
        public ReleaseAsset(int id, string name, int size, string url, string browser_download_url)
        {
            Id = id;
            Name = name;
            Size = size;
            Url = url;
            BrowserDownloadUrl = browser_download_url;
        }

        public readonly int Id;

        public readonly string Name;

        public readonly int Size;

        public readonly string Url;

        public readonly string BrowserDownloadUrl;
    }
}