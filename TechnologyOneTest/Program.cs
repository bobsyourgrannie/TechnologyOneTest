using TechnologyOneTest.BusinessLogic.Implementation;
using TechnologyOneTest.Data;
using TechnologyOneTest.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<NumberToTextService>();
builder.Services.AddSingleton<INumberConverter, NumberConverter>();

var app = builder.Build();

//var weatherService = builder.Services.GetRequiredService<INumberConverter>();
//await weatherService.InitializeWeatherAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
