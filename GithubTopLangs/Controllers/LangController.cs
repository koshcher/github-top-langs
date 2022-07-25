using GithubTopLangs.Models;
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

    public LangController(IConfiguration config)
    {
      github = new(config.GetValue<string>("Github:Token"));
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
        bool includePrivate = true, bool includeOrgs = true, bool includeForks = true, int count = 5)
    {
      // take from query

      IEnumerable<Lang> langs;

      string[]? hideLangs = hide?.Split(',');
      string[]? excludeRepos = exclude?.Split(',');

      langs = await github.CountUserLangs(name, count, includeOrgs, includePrivate, includeForks, hideLangs, excludeRepos);

      string svgCard = svg.CardSvg(langs, background ?? "#3c4043");

      return Content(svgCard, "image/svg+xml");
    }

    [HttpGet("/org")]
    public async Task<IActionResult> Get(
        string name, string? hide, string? background, string? exclude,
        bool includePrivate = true, bool includeForks = true, int count = 5)
    {
      // take from query

      IEnumerable<Lang> langs;

      string[]? hideLangs = hide?.Split(',');
      string[]? excludeRepos = exclude?.Split(',');

      langs = await github.CountOrgLangs(name, count, includePrivate, includeForks, hideLangs, excludeRepos);

      string svgCard = svg.CardSvg(langs, background ?? "#3c4043");

      return Content(svgCard, "image/svg+xml");
    }
  }
}