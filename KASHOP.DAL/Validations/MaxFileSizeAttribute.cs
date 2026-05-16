using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Validations
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
       private readonly int _maxsizeInMB;
        public MaxFileSizeAttribute( int maxSizeInMB) { 
        
            _maxsizeInMB = maxSizeInMB;

        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var sizeInMB = file.Length / (1024 * 1024);
                if (sizeInMB > _maxsizeInMB)
                    return new ValidationResult($"MAx file size is :{_maxsizeInMB}MB ");
            }


            return ValidationResult.Success;
        }
          
    }
}
