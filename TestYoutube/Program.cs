using Telegram.Bot.Polling;
using Telegram.Bot;
using TestYoutube.Services.BackGroundServices;
using TestYoutube.Services;
using TestYoutube.Data;
using TestYoutube.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var token = builder.Configuration["token"];
builder.Services.AddSingleton(new TelegramBotClient(token));

builder.Services.AddHostedService<BotBackgroundService>();
builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddDbContext<BotYoutubeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddSingleton<IUpdateHandler, UpdateHandlerService>();

var app = builder.Build();

app.Run();
