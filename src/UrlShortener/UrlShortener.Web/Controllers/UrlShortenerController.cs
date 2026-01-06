using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Constants;
using UrlShortener.Web.Data;
using UrlShortener.Web.Models;

namespace UrlShortener.Web.Controllers
{
    public class UrlShortenerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string validChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private int len = GlobalVariables.codeLength;
        private string generatedCode = string.Empty;

        public UrlShortenerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(UrlViewModel model)
        {
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Generate(UrlViewModel model)
        {
            var getUrl = GetUrl(model.LongUrl!);

            if (getUrl == null)
            {
                getUrl = new Entities.UrlShortener
                {
                    Id = new Guid(),
                    LongUrl = model.LongUrl!,
                    ShortUrl = GenerateUrl(),
                    Code = generatedCode,
                    CreatedTime = DateTime.Now
                };

                _context.UrlShorteners.Add(getUrl);
                _context.SaveChanges();
            }

            model.Id = getUrl.Id;
            model.LongUrl = getUrl.LongUrl;
            model.ShortUrl = getUrl.ShortUrl;
            model.Code = getUrl.Code;

            return RedirectToAction("Index", model);
        }

        public IActionResult RedirectUrl(string url)
        {
            return Redirect(url);
        }


        private Entities.UrlShortener GetUrl(string url) 
        {
            return _context.UrlShorteners
                .Where(x => x.LongUrl == url)
                .FirstOrDefault()!;
        }

        private string GenerateUrl()
        {
            string currDomain = HttpContext.Request.Host.Value;
            generatedCode = string.Empty;
            var flag = true;

            while (flag)
            {
                for (int i = 0; i < len; i++)
                {
                    var num = new Random().Next(validChar.Length - 1);

                    generatedCode += validChar[num];
                }

                flag = IsExists(generatedCode);
            }

            return $"https://{currDomain}/{generatedCode}";
        }

        private bool IsExists(string code)
        {
            var result = _context.UrlShorteners
                           .AsNoTracking()
                           .FirstOrDefault(x => x.Code == code);

            return (result != null) ? true : false;
        }
    }
}
