using ModernHttpClient;
using Newtonsoft.Json;
using PCLStorage;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitStarsApp
{
    public class GitHubService
    {
        //public List<Repo> GetRepos(int days = 30)
        //{
        //    var backInDays = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd");
        //    List<Repo> repos = null;
        //    var gitUrl = $"http://api.github.com/search/repositories?q=created:%3E={backInDays}+stars:%3E=100&sort=stars";

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36");
        //            var response = client.GetAsync(gitUrl).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = response.Content.ReadAsStringAsync().Result;
        //                var rootObject = JsonConvert.DeserializeObject<RootObject>(result);

        //                repos = rootObject.items.Select(i =>
        //                    new Repo()
        //                    {
        //                        Name = i.full_name,
        //                        Description = i.description,
        //                        Owner = i.owner.login,
        //                        AvatarUrl = i.owner.avatar_url
        //                    }
        //                ).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string s = ex.Message;
        //    }
        //    return repos;
        //}

        
        // Let's go async. The beauty of .NET. Eases on the UI when fetching data over network. No more UI freezing.
        public async Task<List<Repo>> GetReposAsync(int days = 30)
        {
            List<Repo> repos = null;
            var backInDays = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd");
            var gitUrl = $"http://api.github.com/search/repositories?q=created:%3E={backInDays}+stars:%3E=100&sort=stars";

            try
            {
                var localStorage = FileSystem.Current.LocalStorage;
                var localFolder = await localStorage.CreateFolderAsync("Repos", CreationCollisionOption.OpenIfExists);

                if (CrossConnectivity.Current.IsConnected)
                {
                    using (var client = new HttpClient(new NativeMessageHandler()))
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36");
                        var response = await client.GetAsync(gitUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            repos = ParseRepos(result);

                            // GitHub api lets you read only 30 at a time. So we may have to add some paging options here. Fetch more on demand
                            // https://developer.github.com/v3/
                            // Look under pagination header

                            var file = await localFolder.CreateFileAsync($"repo_{days}.json", CreationCollisionOption.ReplaceExisting);
                            await file.WriteAllTextAsync(result);
                        }
                    }
                }
                else
                {
                    var file = await localFolder.GetFileAsync($"repo_{days}.json");
                    var result = await file.ReadAllTextAsync();

                    repos = ParseRepos(result);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return repos;
        }

        private List<Repo> ParseRepos(string repoAsText)
        {
            var root = JsonConvert.DeserializeObject<RootObject>(repoAsText);
            return root.items.Select(item =>
                new Repo()
                {
                    Name = item.full_name,
                    Description = item.description,
                    Url = item.html_url,
                    Owner = item.owner.login,
                    Stars = item.stargazers_count,
                    Language = item.language,
                    AvatarUrl = item.owner.avatar_url
                }).ToList();
        }
    }
}
