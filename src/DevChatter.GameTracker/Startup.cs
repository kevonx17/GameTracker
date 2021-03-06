using AutoMapper;
using DevChatter.GameTracker.Core.Data;
using DevChatter.GameTracker.Core.Model;
using DevChatter.GameTracker.Data.Ef;
using DevChatter.GameTracker.Infra.Bgg;
using DevChatter.GameTracker.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevChatter.GameTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepository, EfCoreRepository>();
            services.AddScoped<IGameDataService, GameDataService>();

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;

                cfg.CreateMap<GameReview, GameReviewViewModel>()
                    .ForMember(x => x.ReviewText,
                        c => c.MapFrom(src => src.Text))
                    .ForMember(x => x.ReviewerName,
                        c => c.MapFrom(src => src.User.UserName));

                cfg.CreateMap<Game, GameViewModel>()
                    .ForMember(x => x.BoardGameGeekTitle,
                        c => c.Ignore())
                    .ForMember(x => x.BoardGameGeekId,
                        c => c.Ignore())
                    .ForMember(x => x.BoardGameGeekLink,
                        c => c.Ignore());
            });

            app.UseMvc();
        }
    }
}
