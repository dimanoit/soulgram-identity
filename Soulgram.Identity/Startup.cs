// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Cronos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using soulgram.identity.BackgroundJob;
using soulgram.identity.Data;
using soulgram.identity.Models;
using Soulgram.Logging;
using Soulgram.Logging.Enums;
using Soulgram.Logging.Models;

namespace soulgram.identity;

public class Startup
{
    public Startup(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddControllersWithViews();
        services.AddEventBus(Configuration);

        services.Configure<SendFailedEventsJobOptions>(Configuration);
        services.AddHostedService<SendFailedEventsJob>();

        AddDbWithIdentity(services);
        AddLogging(services);

        #region TODO add extarnal providers

        /*
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "746627261742-mt7m1er4agl966tech0qmdet0h0bm6ol.apps.googleusercontent.com";
                options.ClientSecret = "-DwH8bhkenPUnqjHVK3mUYrh";
            })
            .AddFacebook(config =>
            {
                //For registration app in facebook https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-5.0
                config.AppId = "Add App Id";
                config.AppSecret = "Add App Secret";
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = "Microsoft Client Id";
                microsoftOptions.ClientSecret = "Microsoft Client Secret";
            })
            .AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = "Consumer Key";
                twitterOptions.ConsumerSecret = "Consumer Secret";
                twitterOptions.RetrieveUserDetails = true;
            });
        */

        #endregion
    }

    public void Configure(IApplicationBuilder app)
    {
        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("MyPolicy");
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }

    private void AddDbWithIdentity(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Identity")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryApiResources(Config.Apis)
            .AddAspNetIdentity<ApplicationUser>();

        services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));

        services.AddLocalApiAuthentication();

        // not recommended for production - you need to store your key material somewhere secure
        builder.AddDeveloperSigningCredential();
    }

    private void AddLogging(IServiceCollection services)
    {
        var loggingSettings = Configuration
            .GetSection("LoggingSettings")
            .Get<LoggingSettings>();

        services.AddLogging(loggingSettings);
    }
}