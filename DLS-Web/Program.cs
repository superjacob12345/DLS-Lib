using System.Globalization;
using DLS_Lib.Repository;
using DLS_Lib.UserStories;

var builder = WebApplication.CreateBuilder(args);

// Datoer vises på dansk (fx "mandag 26. oktober")
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("da-DK");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("da-DK");

// Add services to the container.
builder.Services.AddRazorPages();

// Transient: hver side får sine egne repositories, så to brugere ikke deler forbindelse
builder.Services.AddTransient<AktivitetRepository>();
builder.Services.AddTransient<SpejderRepository>();
builder.Services.AddTransient<LederRepository>();
builder.Services.AddTransient<TegnRepository>();
builder.Services.AddTransient<TilmeldingRepository>();

// Session bruges til at huske hvem der er logget ind
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===== Eksempeldata, så der er noget at se første gang siden startes =====
// Alle brugere har adgangskoden "1234"
TegnRepository tegnRepo = app.Services.GetRequiredService<TegnRepository>();
LederRepository lederRepo = app.Services.GetRequiredService<LederRepository>();
SpejderRepository spejderRepo = app.Services.GetRequiredService<SpejderRepository>();
AktivitetRepository aktivitetRepo = app.Services.GetRequiredService<AktivitetRepository>();
TilmeldingRepository tilmeldingRepo = app.Services.GetRequiredService<TilmeldingRepository>();

// Data indsættes kun hvis databasen er tom
if (lederRepo.HentAlle().Count == 0)
{
    Tegn foerstehjaelp = new Tegn("Førstehjælp");
    Tegn baalmester = new Tegn("Bålmester");
    Tegn svoemmer = new Tegn("Svømmer");
    Tegn knob = new Tegn("Knob og reb");
    Tegn kano = new Tegn("Kanosejler");
    tegnRepo.Opret(foerstehjaelp);
    tegnRepo.Opret(baalmester);
    tegnRepo.Opret(svoemmer);
    tegnRepo.Opret(knob);
    tegnRepo.Opret(kano);

    Leder mette = new Leder("Mette Hansen", "mette", "1234");
    Leder jonas = new Leder("Jonas Berg", "jonas", "1234");
    Leder aisha = new Leder("Aisha Khan", "aisha", "1234");
    lederRepo.Opret(mette);
    lederRepo.Opret(jonas);
    lederRepo.Opret(aisha);

    Spejder ida = new Spejder(0, "Ida Larsen", "ida", "1234", new DateOnly(2014, 3, 12)) { TegnIds = new List<int> { baalmester.Id, svoemmer.Id } };
    Spejder oliver = new Spejder(0, "Oliver Nielsen", "oliver", "1234", new DateOnly(2017, 8, 2));
    Spejder noah = new Spejder(0, "Noah Pedersen", "noah", "1234", new DateOnly(2011, 1, 20)) { TegnIds = new List<int> { foerstehjaelp.Id, svoemmer.Id, kano.Id } };
    Spejder freja = new Spejder(0, "Freja Madsen", "freja", "1234", new DateOnly(2013, 11, 5)) { TegnIds = new List<int> { knob.Id } };
    spejderRepo.Opret(ida);
    spejderRepo.Opret(oliver);
    spejderRepo.Opret(noah);
    spejderRepo.Opret(freja);

    // Lejren starter førstkommende mandag
    int dageTilMandag = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
    if (dageTilMandag == 0)
    {
        dageTilMandag = 6;
    }
    DateTime mandag = DateTime.Today.AddDays(dageTilMandag);

    Aktivitet baal = new Aktivitet("Mad over bål", mandag.AddHours(17), 12, 8, "Bålpladsen", "Vi laver snobrød og pandekager over åben ild. Medbring egen kop!")
    {
        LederIds = new List<int> { mette.Id }
    };
    Aktivitet ridetur = new Aktivitet("Ridetur på Hedegården", mandag.AddDays(1).AddHours(10), 6, 10, "Hedegården", "Besøg gården, mød dyrene og tag en tur på hesteryg rundt om markerne.")
    {
        LederIds = new List<int> { jonas.Id }
    };
    Aktivitet vandhul = new Aktivitet("Fang dyr i vandhullet", mandag.AddDays(1).AddHours(14), 15, 6, "Vandhullet", "Med net og spande undersøger vi hvad der lever i vandet – haletudser, vandkalve og meget mere.")
    {
        LederIds = new List<int> { aisha.Id }
    };
    Aktivitet kanotur = new Aktivitet("Kanotur på søen", mandag.AddDays(2).AddHours(9).AddMinutes(30), 8, 12, "Hedeland Sø", "En hel formiddag på vandet. Kræver at man kan svømme og har kanotegnet.")
    {
        LederIds = new List<int> { jonas.Id, aisha.Id },
        KraevedeTegnIds = new List<int> { svoemmer.Id, kano.Id }
    };
    Aktivitet svaevebane = new Aktivitet("Byg en svævebane", mandag.AddDays(3).AddHours(13), 10, 11, "Skoven", "Vi bygger en svævebane mellem træerne med reb og knob.")
    {
        LederIds = new List<int> { aisha.Id },
        KraevedeTegnIds = new List<int> { knob.Id }
    };
    Aktivitet natloeb = new Aktivitet("Natløb med lommelygter", mandag.AddDays(3).AddHours(21), 20, 11, "Lejrens indgang", "Find posterne i mørket og løs opgaverne undervejs.")
    {
        LederIds = new List<int> { mette.Id, jonas.Id }
    };
    Aktivitet lejrbaal = new Aktivitet("Lejrbål og sang", mandag.AddDays(4).AddHours(20), 200, 0, "Det store bål", "Ugens afslutning med sange, sketches og skumfiduser for hele lejren.")
    {
        LederIds = new List<int> { mette.Id, jonas.Id, aisha.Id }
    };
    aktivitetRepo.Opret(baal);
    aktivitetRepo.Opret(ridetur);
    aktivitetRepo.Opret(vandhul);
    aktivitetRepo.Opret(kanotur);
    aktivitetRepo.Opret(svaevebane);
    aktivitetRepo.Opret(natloeb);
    aktivitetRepo.Opret(lejrbaal);

    // Et par tilmeldinger, så der er noget at se
    tilmeldingRepo.Tilmeld(ida, baal);
    tilmeldingRepo.Tilmeld(ida, vandhul);
    tilmeldingRepo.Tilmeld(noah, kanotur);
    tilmeldingRepo.Tilmeld(freja, svaevebane);
}

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
