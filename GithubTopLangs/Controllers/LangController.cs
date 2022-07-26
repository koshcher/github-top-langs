using GithubTopLangs.Models;
using GithubTopLangs.Services.Caching;
using GithubTopLangs.Services.Github;
using GithubTopLangs.Services.Svg;
using Microsoft.AspNetCore.Mvc;

namespace GithubTopLangs.Controllers
{
  //[Route("api/[controller]")]
  [ApiController]
  public class LangController : ControllerBase
  {
    private GithubService github;
    private SvgService svg = new();
    private CacheService cache;

    public LangController(IConfiguration config)
    {
      github = new(config.GetValue<string>("TOKEN"));
      cache = CacheService.Instance;
    }

    /// <param name="name">github username</param>
    /// <param name="hide">comma separated names of langs to not count</param>
    /// <param name="background">hex backgroud color</param>
    /// <param name="exclude">comma separated names of repos to not count</param>
    /// <param name="includePrivate">is to count private repos</param>
    /// <param name="includeOrgs">is to count organizations repos</param>
    /// <param name="count">count of langs</param>
    [HttpGet("/user")]
    public async Task<IActionResult> Get(
        string name, string? hide, string? background, string? exclude,
        bool includePrivate = true, bool includeOrgs = true, bool includeForks = true, int count = 5
      )
    {
      string svgCard = cache.GetCachedSvg(name) ?? svg.FirstAskCard(background ?? "#3c4043");

      // make new request
      Task.Run(async () =>
      {
        string[]? hideLangs = hide?.Split(',');
        string[]? excludeRepos = exclude?.Split(',');

        IEnumerable<Lang> langs = await github.CountUserLangs(name, count, includeOrgs, includePrivate, includeForks, hideLangs, excludeRepos);

        string svgCard = svg.LangCard(langs, background ?? "#3c4043");
        cache.CacheSvg(name, svgCard);
      });

      return Content(svgCard, "image/svg+xml");
    }

    [HttpGet("/org")]
    public async Task<IActionResult> Get(
        string name, string? hide, string? background, string? exclude,
        bool includePrivate = true, bool includeForks = true, int count = 5)
    {
      string svgCard = cache.GetCachedSvg(name) ?? svg.FirstAskCard(background ?? "#3c4043");

      Task.Run(async () =>
      {
        string[]? hideLangs = hide?.Split(',');
        string[]? excludeRepos = exclude?.Split(',');

        IEnumerable<Lang> langs = await github.CountOrgLangs(name, count, includePrivate, includeForks, hideLangs, excludeRepos);

        string svgCard = svg.LangCard(langs, background ?? "#3c4043");
        cache.CacheSvg(name, svgCard);
      });

      return Content(svgCard, "image/svg+xml");
    }
  }
}