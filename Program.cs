using Game_Realtime.Hubs;
using Game_Realtime.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<MythicEmpireHub>();
builder.Services.AddSingleton<IGameService,GameService>();
builder.Services.AddSingleton<IUserMatchingService,UserMatchingService>();
var app = builder.Build();

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

app.UseAuthorization();

// app.MapRazorPages();
app.MapHub<MythicEmpireHub>("/realtimeHub");

app.Run();
