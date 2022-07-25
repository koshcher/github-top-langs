using Octokit;

namespace GithubTopLangs.Services.Github.Counters
{
  public interface ILangCounter
  {
    public Task Count(Repository repo);
  }

  public class LangCounter : ILangCounter
  {
    private readonly Action<string, long> addLang;
    private readonly GitHubClient client;

    public LangCounter(GitHubClient client, Action<string, long> addLang)
    {
      this.addLang = addLang;
      this.client = client;
    }

    public async Task Count(Repository repo)
    {
      var langs = await client.Repository.GetAllLanguages(repo.Id);
      foreach (var lang in langs)
      {
        addLang(lang.Name, lang.NumberOfBytes);
      }
    }
  }
}