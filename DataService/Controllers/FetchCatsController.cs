using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace asp_dotnet_core_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchCatsController : ControllerBase
    {
        private static string[] CatsList = new[]
        {
            "Cat1", "Cat2", "Cat3"
        };

        [HttpGet("[action]")]
        public IEnumerable<Cats> GetCats()
        {
            var rng = new Random();
            yield return new Cats
            {
                SelectedCat = CatsList[rng.Next(CatsList.Length)]
            };
        }

        public class Cats
        {
            public string SelectedCat { get; set; }
        }
    }
}