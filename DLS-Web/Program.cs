//using DLS_Lib.Repository;
//using DLS_Web.Hjaelpere;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Vi validerer selv felterne i OnPost, så tomme lister (fx ingen tegn valgt) ikke giver fejl,
//// og tomme tekstfelter bliver til "" i stedet for null
//builder.Services.AddRazorPages()
//    .AddMvcOptions(o =>
//    {
//        o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
//        o.ModelMetadataDetailsProviders.Add(new TommeTekster());
//    });

//builder.Services.AddSingleton<AktivitetRepository>();
//builder.Services.AddSingleton<SpejderRepository>();
//builder.Services.AddSingleton<LederRepository>();
//builder.Services.AddSingleton<TegnRepository>();
//builder.Services.AddSingleton<TilmeldingRepository>();

//// Session bruges til at huske hvem der er logget ind
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromHours(2);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();

//// Eksempeldata, så der er noget at se når siden startes
//Testdata.Opret(app.Services);

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.UseRouting();

//app.UseSession();

//app.UseAuthorization();

//app.MapStaticAssets();
//app.MapRazorPages()
//   .WithStaticAssets();

//app.Run();
