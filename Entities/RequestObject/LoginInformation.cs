﻿using System.ComponentModel.DataAnnotations;

namespace Entities.RequestObject
{
    public class LoginInformation
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
