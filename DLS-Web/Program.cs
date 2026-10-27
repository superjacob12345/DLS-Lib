using DLS_Lib.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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
