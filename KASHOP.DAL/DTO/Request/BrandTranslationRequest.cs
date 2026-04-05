using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class BrandTranslationRequest
    {
        public string Language { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}
