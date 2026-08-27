namespace AspnetCoreMvcFull
{
  public class Startup
  {
    readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
    public IConfiguration Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
      services.AddSession();
      services.AddHttpContextAccessor();

      services.AddHttpClient("DigitalSign", client =>
      {
        client.BaseAddress = new Uri(Configuration["DigitalSignApi:BaseUrl"]);
      });

      services.AddCors(options =>
      {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          builder =>
                          {
                            builder.WithOrigins("https://localhost",
                            "http://www.insure.co.th")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                          });
      });
      /* Toto Win Authen
      //services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
      //services.AddAuthorization(options =>
      //{
      //    // By default, all incoming requests will be authorized according to the default policy.
      //    options.FallbackPolicy = options.DefaultPolicy;
      //});
      */

      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;   // consent required
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      //In-Memory 19/03/2021

      services.AddDistributedMemoryCache();
      services.AddSession(options => {
        options.IdleTimeout = TimeSpan.FromMinutes(60);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
      });

    }
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }
      app.Use(async (context, next) =>
      {
        await next.Invoke();

        //After going down the pipeline check if we 404'd. 
        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
          await context.Response.WriteAsync("Woops! We 404'd");
        }
      });

      app.UseHttpsRedirection();
      app.UseStaticFiles(new StaticFileOptions
      {
        OnPrepareResponse = ctx =>
        {
          // Page-specific JS (prapproval.js, prresultrequest.js, etc.) changes often
          // during development and is only cache-busted via a manual ?v= query string
          // on the <script> tag. Tell every cache layer (browser, IIS Express kernel
          // cache) to never serve a stale copy, regardless of that query string.
          // Minified vendor libraries (jquery.min.js, datatables.min.js, ...) are
          // excluded since they're stable and safe to cache normally.
          if (ctx.File.Name.EndsWith(".js") && !ctx.File.Name.EndsWith(".min.js"))
          {
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers["Pragma"] = "no-cache";
            ctx.Context.Response.Headers["Expires"] = "0";
          }
        }
      });
      app.UseRouting();
      app.UseSession(); //19/03/2021

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSession();

      app.UseCors(MyAllowSpecificOrigins);
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          //pattern: "{controller=Account}/{action=Login}/{id?}");

          //pattern: "{controller=CTLAdmin}/{action=PRResultRequest}/{id?}");
          pattern: "{controller=Dashboards}/{action=Index}/{id?}");

          //pattern: "{controller=SGA}/{action=PRDRecordSheet}/{id?}");

          //pattern: "{controller=ReduceItem}/{action=Index}/{id?}");
          //pattern: "{controller=Monitor}/{action=Index}/{id?}");
          //pattern: "{controller=AdjustMinMax}/{action=Index}/{id?}");
      });

    }

  }
}
