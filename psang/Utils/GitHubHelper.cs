/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Psang.Utils
{
    public static class GitHubHelper
    {
        private static readonly string owner = "imjustprism";
        private static readonly string repo = "psang";

        public static string GetLatestVersion()
        {
            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.UserAgent = "psang-updater";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(GitHubRelease));
                var release = (GitHubRelease)serializer.ReadObject(stream);
                return release.Tag_name.TrimStart('v');
            }
        }

        public static string GetLatestDownloadUrl()
        {
            string apiUrl = $"https://api.github.com/repos/{owner} / {repo}/releases/latest";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.UserAgent = "psang-updater";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(GitHubRelease));
                var release = (GitHubRelease)serializer.ReadObject(stream);
                foreach (var asset in release.Assets)
                {
                    if (asset.Name.EndsWith(".exe") || asset.Name.EndsWith(".zip"))
                    {
                        return asset.Browser_download_url;
                    }
                }
                throw new Exception("No suitable asset found for download.");
            }
        }

        public static void DownloadFile(string url, string destinationPath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, destinationPath);
            }
        }

        [DataContract]
        private class GitHubRelease
        {
            [DataMember(Name = "tag_name")]
            public string Tag_name { get; set; }

            [DataMember(Name = "assets")]
            public GitHubAsset[] Assets { get; set; }
        }

        [DataContract]
        private class GitHubAsset
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "browser_download_url")]
            public string Browser_download_url { get; set; }
        }
    }
}
