using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Models
{
    public class Inspection
    {
        [Key]
        public int InspectionId { get; set; }

        [Required]
        public int ProjectorId { get; set; }

        [Required]
        public bool Lens { get; set; }

        [Required]
        public bool Button { get; set; }

        [Required]
        public bool Power { get; set; }

        [Required]
        public bool Speaker { get; set; }

        [Required]
        [Range(0, 100)]
        public int Temperature { get; set; }

        //  System-generated → must be nullable
        public string? Result { get; set; }

        // System-generated → must be nullable
        public DateTime? CheckedAt { get; set; }
    }
}