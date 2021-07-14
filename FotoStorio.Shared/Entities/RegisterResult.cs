using System.Collections.Generic;

namespace FotoStorio.Shared.Entities
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}