using Octokit;

namespace GithubTopLangs.Services.Github.Counters
{
  public abstract class LangDecorator : ILangCounter
  {
    protected ILangCounter counter;

    protected LangDecorator(ILangCounter counter) => this.counter = counter;

    public virtual async Task Count(Repository repo) => await counter.Count(repo);
  }

  public class NoForkDecorator : LangDecorator
  {
    public NoForkDecorator(ILangCounter counter) : base(counter)
    { }

    public override async Task Count(Repository repo)
    {
      if (!repo.Fork) await base.Count(repo);
    }
  }

  public class PublicDecorator : LangDecorator
  {
    public PublicDecorator(ILangCounter counter) : base(counter)
    { }

    public override async Task Count(Repository repo)
    {
      if (!repo.Private) await base.Count(repo);
    }
  }

  public class RepoExcludeDecorator : LangDecorator
  {
    private readonly string[] exclude;

    public RepoExcludeDecorator(ILangCounter counter, string[] exclude) : base(counter)
    {
      this.exclude = exclude;
    }

    public override async Task Count(Repository repo)
    {
      if (!exclude.Contains(repo.Name)) await base.Count(repo);
    }
  }
}