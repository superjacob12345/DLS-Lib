using System.ComponentModel.DataAnnotations;

namespace DLS_Lib.UserStories
{
    // Egen valideringsregel: en fødselsdato må ikke ligge i fremtiden
    // og må ikke være mere end 100 år tilbage.
    public class FoedselsdatoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly dato)
            {
                DateOnly idag = DateOnly.FromDateTime(DateTime.Today);
                if (dato > idag)
                {
                    return new ValidationResult("Fødselsdatoen kan ikke ligge i fremtiden");
                }
                if (dato < idag.AddYears(-100))
                {
                    return new ValidationResult("Fødselsdatoen kan ikke være mere end 100 år tilbage");
                }
            }
            return ValidationResult.Success;
        }
    }
}
