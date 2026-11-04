using DLS_Lib.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        // Felter uden [Required] (fx Sted og Beskrivelse) må gerne være tomme
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

        // Danske fejlbeskeder når en værdi ikke kan læses (fx bogstaver i et tal-felt)
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(felt => "Feltet skal udfyldes");
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(felt => "Feltet skal udfyldes");
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((vaerdi, felt) => "\"" + vaerdi + "\" er ikke en gyldig værdi for " + felt);
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(vaerdi => "\"" + vaerdi + "\" er ikke en gyldig værdi");
        options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "Skriv et tal");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(felt => felt + " skal være et tal");
    });

// Repositories
builder.Services.AddTransient<AktivitetRepository>();
builder.Services.AddTransient<SpejderRepository>();
builder.Services.AddTransient<LederRepository>();
builder.Services.AddTransient<TegnRepository>();
builder.Services.AddTransient<TilmeldingRepository>();

// Session bruges til at huske hvem der er logget ind
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
