﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Perficient.OpenShift.Workshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchCatsController : ControllerBase
    {
        private static string[] CatsList = new[]
        {
            "Calico", "Tuxedo", "Ragdoll", "Persian", "Burmese"
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