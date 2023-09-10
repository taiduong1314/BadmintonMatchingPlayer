using System;
using System.Collections.Generic;

namespace BadmintonMatching.Models
{
    public partial class Wishlist
    {
        public int? IdUser { get; set; }
        public int? IdPost { get; set; }

        public virtual Post? IdPostNavigation { get; set; }
        public virtual User? IdUserNavigation { get; set; }
    }
}
