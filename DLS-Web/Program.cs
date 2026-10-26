using System.Globalization;
using DLS_Lib.Repository;
using DLS_Lib.UserStories;

var builder = WebApplication.CreateBuilder(args);

// Datoer vises på dansk (fx "mandag 26. oktober")
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("da-DK");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("da-DK");

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<AktivitetRepository>();
builder.Services.AddSingleton<SpejderRepository>();
builder.Services.AddSingleton<LederRepository>();
builder.Services.AddSingleton<TegnRepository>();
builder.Services.AddSingleton<TilmeldingRepository>();

// Session bruges til at huske hvem der er logget ind
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===== Eksempeldata, så der er noget at se når siden startes =====
// Alle brugere har adgangskoden "1234"
TegnRepository tegnRepo = app.Services.GetRequiredService<TegnRepository>();
LederRepository lederRepo = app.Services.GetRequiredService<LederRepository>();
SpejderRepository spejderRepo = app.Services.GetRequiredService<SpejderRepository>();
AktivitetRepository aktivitetRepo = app.Services.GetRequiredService<AktivitetRepository>();
TilmeldingRepository tilmeldingRepo = app.Services.GetRequiredService<TilmeldingRepository>();

// Tegn får Id 1-5
tegnRepo.Opret(new Tegn("Førstehjælp"));
tegnRepo.Opret(new Tegn("Bålmester"));
tegnRepo.Opret(new Tegn("Svømmer"));
tegnRepo.Opret(new Tegn("Knob og reb"));
tegnRepo.Opret(new Tegn("Kanosejler"));

// Ledere får Id 1-3
lederRepo.Opret(new Leder("Mette Hansen", "mette", "1234"));
lederRepo.Opret(new Leder("Jonas Berg", "jonas", "1234"));
lederRepo.Opret(new Leder("Aisha Khan", "aisha", "1234"));

// Spejdere får Id 1-4
spejderRepo.Opret(new Spejder(0, "Ida Larsen", "ida", "1234", new DateOnly(2014, 3, 12)) { TegnIds = new List<int> { 2, 3 } });
spejderRepo.Opret(new Spejder(0, "Oliver Nielsen", "oliver", "1234", new DateOnly(2017, 8, 2)));
spejderRepo.Opret(new Spejder(0, "Noah Pedersen", "noah", "1234", new DateOnly(2011, 1, 20)) { TegnIds = new List<int> { 1, 3, 5 } });
spejderRepo.Opret(new Spejder(0, "Freja Madsen", "freja", "1234", new DateOnly(2013, 11, 5)) { TegnIds = new List<int> { 4 } });

// Lejren starter førstkommende mandag
int dageTilMandag = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
if (dageTilMandag == 0)
{
    dageTilMandag = 7;
}
DateTime mandag = DateTime.Today.AddDays(dageTilMandag);

aktivitetRepo.Opret(new Aktivitet("Mad over bål", mandag.AddHours(17), 12, 8)
{
    Sted = "Bålpladsen",
    Beskrivelse = "Vi laver snobrød og pandekager over åben ild. Medbring egen kop!",
    LederIds = new List<int> { 1 }
});
aktivitetRepo.Opret(new Aktivitet("Ridetur på Hedegården", mandag.AddDays(1).AddHours(10), 6, 10)
{
    Sted = "Hedegården",
    Beskrivelse = "Besøg gården, mød dyrene og tag en tur på hesteryg rundt om markerne.",
    LederIds = new List<int> { 2 }
});
aktivitetRepo.Opret(new Aktivitet("Fang dyr i vandhullet", mandag.AddDays(1).AddHours(14), 15, 6)
{
    Sted = "Vandhullet",
    Beskrivelse = "Med net og spande undersøger vi hvad der lever i vandet – haletudser, vandkalve og meget mere.",
    LederIds = new List<int> { 3 }
});
aktivitetRepo.Opret(new Aktivitet("Kanotur på søen", mandag.AddDays(2).AddHours(9).AddMinutes(30), 8, 12)
{
    Sted = "Hedeland Sø",
    Beskrivelse = "En hel formiddag på vandet. Kræver at man kan svømme og har kanotegnet.",
    LederIds = new List<int> { 2, 3 },
    KraevedeTegnIds = new List<int> { 3, 5 }
});
aktivitetRepo.Opret(new Aktivitet("Byg en svævebane", mandag.AddDays(3).AddHours(13), 10, 11)
{
    Sted = "Skoven",
    Beskrivelse = "Vi bygger en svævebane mellem træerne med reb og knob.",
    LederIds = new List<int> { 3 },
    KraevedeTegnIds = new List<int> { 4 }
});
aktivitetRepo.Opret(new Aktivitet("Natløb med lommelygter", mandag.AddDays(3).AddHours(21), 20, 11)
{
    Sted = "Lejrens indgang",
    Beskrivelse = "Find posterne i mørket og løs opgaverne undervejs.",
    LederIds = new List<int> { 1, 2 }
});
aktivitetRepo.Opret(new Aktivitet("Lejrbål og sang", mandag.AddDays(4).AddHours(20), 200, 0)
{
    Sted = "Det store bål",
    Beskrivelse = "Ugens afslutning med sange, sketches og skumfiduser for hele lejren.",
    LederIds = new List<int> { 1, 2, 3 }
});

// Et par tilmeldinger, så der er noget at se
tilmeldingRepo.Tilmeld(spejderRepo.HentEn(1)!, aktivitetRepo.HentEn(1)!);
tilmeldingRepo.Tilmeld(spejderRepo.HentEn(1)!, aktivitetRepo.HentEn(3)!);
tilmeldingRepo.Tilmeld(spejderRepo.HentEn(3)!, aktivitetRepo.HentEn(4)!);
tilmeldingRepo.Tilmeld(spejderRepo.HentEn(4)!, aktivitetRepo.HentEn(5)!);

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
