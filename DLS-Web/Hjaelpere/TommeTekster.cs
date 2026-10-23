using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DLS_Web.Hjaelpere
{
    // Normalt laver ASP.NET et tomt tekstfelt om til null.
    // Vores klasser bruger "" som tom tekst, så det slår vi fra.
    public class TommeTekster : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.ModelType == typeof(string))
            {
                context.DisplayMetadata.ConvertEmptyStringToNull = false;
            }
        }
    }
}
