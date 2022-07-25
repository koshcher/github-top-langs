using GithubTopLangs.Models;
using GithubTopLangs.Services.Github.Counters;
using Octokit;

namespace GithubTopLangs.Services.Github
{
  public class GithubService
  {
    protected GitHubClient client = new(new ProductHeaderValue("roman-koshchei"));

    public GithubService(IConfiguration config)
    {
      var tokenAuth = new Credentials(config.GetValue<string>("Github:Token"));
      client.Credentials = tokenAuth;
    }

    /// <param name="username"></param>
    /// <param name="count"></param>
    /// <param name="isOrgs">is include orgs</param>
    /// <param name="isPrivate">is include private repos</param>
    /// <param name="hide">list of langs that should be not counted</param>
    public async Task<IEnumerable<Lang>> CountUserLangs(
        string username, int count, bool isOrgs, bool isPrivate,
        bool includeForks,
        string[]? hide = null, string[]? excludeRepos = null)
    {
      Dictionary<string, long> langs = new();

      Action<string, long> addLang = hide == null
          ? ((name, num) => ToDictionary(name, num, ref langs))
          : ((name, num) => HideToDictionary(name, num, ref langs, hide));

      // build counter chain
      ILangCounter langCounter = new LangCounter(client, addLang);
      if (!isPrivate) langCounter = new PublicDecorator(langCounter);
      if (excludeRepos != null) langCounter = new RepoExcludeDecorator(langCounter, excludeRepos);
      if (!includeForks) langCounter = new NoForkDecorator(langCounter);

      ProfileCounter profileCounter = isOrgs
          ? new UserOrgCounter(client, langCounter)
          : new UserCounter(client, langCounter);

      // run counter chain
      await profileCounter.Count(username);

      return PrepareLangs(langs, count);
    }

    public async Task<IEnumerable<Lang>> CountOrgLangs(string name, int count, bool isPrivate, bool includeForks, string[]? hide = null, string[]? excludeRepos = null)
    {
      Dictionary<string, long> langs = new();

      Action<string, long> addLang = hide == null
          ? ((langName, langCount) => ToDictionary(langName, langCount, ref langs))
          : ((langName, langCount) => HideToDictionary(langName, langCount, ref langs, hide));

      // build counter chain
      ILangCounter langCounter = new LangCounter(client, addLang);
      if (!isPrivate) langCounter = new PublicDecorator(langCounter);
      if (excludeRepos != null) langCounter = new RepoExcludeDecorator(langCounter, excludeRepos);
      if (!includeForks) langCounter = new NoForkDecorator(langCounter);

      ProfileCounter profileCounter = new OrgCounter(client, langCounter);
      // run counter chain
      await profileCounter.Count(name);

      return PrepareLangs(langs, count);
    }

    private void HideToDictionary(string name, long count, ref Dictionary<string, long> dict, string[] hide)
    {
      if (!hide.Contains(name.ToLower())) ToDictionary(name, count, ref dict);
    }

    private LinkedList<Lang> PrepareLangs(Dictionary<string, long> langsDictionary, int count)
    {
      var langsInCount = langsDictionary.OrderByDescending(lang => lang.Value).Take(count);
      long sum = langsInCount.Sum(lang => lang.Value);

      LinkedList<Lang> langsList = new();
      foreach (var lang in langsInCount)
      {
        langsList.AddLast(new Lang { Name = lang.Key, Percent = lang.Value * 1.0 / sum });
      }
      return langsList;
    }

    private void ToDictionary(string name, long count, ref Dictionary<string, long> dictionary)
    {
      if (dictionary.ContainsKey(name)) dictionary[name] += count;
      else dictionary[name] = count;
    }
  }
}