using FluentValidation;
using MusicPlaylistAPI.Mappers;
using MusicPlaylistAPI.Repositories;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services;
using MusicPlaylistAPI.Services.Interface;
using MusicPlaylistAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(PlaylistProfile));
builder.Services.AddAutoMapper(typeof(MusicProfile));
builder.Services.AddAutoMapper(typeof(CommentProfile));
builder.Services.AddAutoMapper(typeof(FollowProfile));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<IMusicRepository, MusicRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFollowService, FollowService>();

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PlaylistValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MusicValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CommentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FollowValidator>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
