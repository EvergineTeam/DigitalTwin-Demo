using CompressedStaticFiles;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddCompressedStaticFiles();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
////app.UseStaticFiles(new StaticFileOptions
////{
////    ServeUnknownFileTypes = true,
////});
var contentTypeProvider = new FileExtensionContentTypeProvider();
var evergineExtensions = new[] { ".weptx", ".wepsn", ".wepsc", ".wepsp", ".weprl", ".weprp", ".weppp", ".wepmd", ".wepmt", ".wepfb", ".wepfx", ".wepprf" };
foreach (var evergineExtension in evergineExtensions)
{
    contentTypeProvider.Mappings.Add(evergineExtension, "application/octet-stream");
}
app.UseCompressedStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    ContentTypeProvider = contentTypeProvider
});

app.UseRouting();


app.MapRazorPages();
//app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
