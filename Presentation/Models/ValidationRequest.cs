﻿using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class ValidationRequest
{
    [Required]
    public string AccessToken { get; set; } = null!;
    
    [Required]
    public string UserId { get; set; } = null!;


}
