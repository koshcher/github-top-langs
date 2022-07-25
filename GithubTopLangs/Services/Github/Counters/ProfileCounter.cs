using Octokit;

namespace GithubTopLangs.Services.Github.Counters
{
  public abstract class ProfileCounter
  {
    protected GitHubClient client;
    protected ILangCounter langCounter;

    protected ProfileCounter(GitHubClient client, ILangCounter langCounter)
    {
      this.client = client;
      this.langCounter = langCounter;
    }

    public abstract Task Count(string name);
  }

  public class UserCounter : ProfileCounter
  {
    public UserCounter(GitHubClient client, ILangCounter langCounter) : base(client, langCounter)
    { }

    public override async Task Count(string name)
    {
      var repos = await client.Repository.GetAllForUser(name);
      foreach (var repo in repos)
      {
        await langCounter.Count(repo);
      }
    }
  }

  public class OrgCounter : ProfileCounter
  {
    public OrgCounter(GitHubClient client, ILangCounter langCounter) : base(client, langCounter)
    {
    }

    public override async Task Count(string name)
    {
      var repos = await client.Repository.GetAllForOrg(name);
      foreach (var repo in repos)
      {
        await langCounter.Count(repo);
      }
    }
  }

  public class UserOrgCounter : ProfileCounter
  {
    private readonly UserCounter userCounter;

    public UserOrgCounter(GitHubClient client, ILangCounter langCounter) : base(client, langCounter)
    {
      userCounter = new(client, langCounter);
    }

    public override async Task Count(string name)
    {
      // call count for urer profile
      await userCounter.Count(name);

      var orgs = await client.Organization.GetAllForUser(name);
      foreach (var org in orgs)
      {
        var repos = await client.Repository.GetAllForOrg(org.Login);
        foreach (var repo in repos)
        {
          await langCounter.Count(repo);
        }
      }
    }
  }
}