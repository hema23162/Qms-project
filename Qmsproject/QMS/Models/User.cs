using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Models;

public class User
{
     [Key]
     public int UserId { get; set; }
     [Required]
     [EmailAddress]
     [MaxLength(100)]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(50)]

    public string Password { get; set; }
    
 [Required]
 [MaxLength(20)]

    public string Role { get; set; }

}
