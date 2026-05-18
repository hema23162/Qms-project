using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Models;

public class Projector
{
 [Key]
public int ProjectorId { get; set; }   // Unique ID

    [Required]
    [MaxLength(20)]

    public string BatchNumber { get; set; } // Manufacturing batch

}
