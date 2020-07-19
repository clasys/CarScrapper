﻿using CarScrapper.Core;
using System.ComponentModel.DataAnnotations;

namespace CarScraper.Web.API.Models
{
    public class CarSearch
    {
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public DealerType DealerType { get; set; }
        public bool IsLoaner { get; set; }
        public Regions Region { get; set; }
    }
}
