﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ResponseObject
{
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public List<Ward> Wards { get; set; }
        // Các thông tin khác về quận
    }
}
