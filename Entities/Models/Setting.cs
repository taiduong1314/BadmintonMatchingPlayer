using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum SettingType
    {
        BookingSetting = 1,
        PostingSetting = 2,
        NumberPostFree = 3,
        BoostPost=4,
    }
    public class Setting
    {
        public int SettingId { get; set; }
        public string? SettingName { get; set; }
        public decimal? SettingAmount { get; set; }
    }
}
