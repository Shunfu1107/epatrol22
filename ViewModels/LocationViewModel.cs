using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPortalV8.Models.Epatrol;

namespace EPatrol.ViewModels
{
    public class LocationViewModel
    {
        public List<Location> Locations { get; set; } = new List<Location>();
    }
}