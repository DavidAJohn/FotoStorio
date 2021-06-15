using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoStorio.Shared.Models
{
    public class Brand : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
