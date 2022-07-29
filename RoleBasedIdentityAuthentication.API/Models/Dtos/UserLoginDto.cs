﻿using System.ComponentModel.DataAnnotations;

namespace RoleBasedIdentityAuthentication.API.Models.Dtos;
public class UserLoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
