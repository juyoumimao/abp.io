using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookStore.EntityFrameworkCore;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace BookStore;
//当前模块要依赖的模块 
[DependsOn(
    //typeof(BookStoreHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(BookStoreApplicationModule),
    typeof(BookStoreEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class BookStoreHttpApiHostModule : AbpModule
{
    //预配置
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        //PreConfigure<OpenIddictBuilder>(builder =>
        //{
        //    builder.AddValidation(options =>
        //    {
        //        options.AddAudiences("BookStore");
        //        options.UseLocalServer();
        //        options.UseAspNetCore();
        //    });
        //});
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureAuthentication(context);//配置身份认证
        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureConventionalControllers();
        ConfigureVirtualFileSystem(context);//虚拟文件
        ConfigureCors(context, configuration);//配置跨域
        ConfigureSwaggerServices(context, configuration);//Swagger文档
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        //context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        //context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        //{
        //    options.IsDynamicClaimsEnabled = true;
        //});

        var configuration = context.Services.GetConfiguration();

        context.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
                .AddJwtBearer(
                option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        //是否验证发行人
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtConfig:Bearer:Issuer"],//发行人

                        //是否验证受众人
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtConfig:Bearer:Audience"],//受众人

                        //是否验证密钥
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Bearer:SecurityKey"])),

                        ValidateLifetime = true, //验证生命周期

                        RequireExpirationTime = true, //过期时间

                        ClockSkew = TimeSpan.FromSeconds(30)   //平滑过期偏移时间
                    };
                }
            );
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            //options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            //options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<BookStoreDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}BookStore.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<BookStoreDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}BookStore.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<BookStoreApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}BookStore.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<BookStoreApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}BookStore.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(BookStoreApplicationModule).Assembly);
        });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "基础接口", Version = "v1" });
            options.SwaggerDoc("v2", new OpenApiInfo { Title = "业务接口", Version = "v2" });

            options.DocInclusionPredicate((doc, desc) =>
            {
                return desc.GroupName == doc;
            });


            //开启权限小锁
            options.OperationFilter<AddResponseHeadersFilter>();
            options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            options.CustomSchemaIds(type => type.FullName);

            //给参数设置默认值
            //options.SchemaFilter<SchemaFilter>();

            //在header中添加token，传递到后台
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格) \"",
                Name = "Authorization",//jwt默认的参数名称
                In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                Type = SecuritySchemeType.ApiKey
            });

            //就是这里！！！！！！！！！
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = Path.Combine(basePath, "BookStore.Application.xml");//这个就是刚刚配置的xml文件名
            options.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改
        });

    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray() ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        //app.UseAbpOpenIddictValidation();

        //if (MultiTenancyConsts.IsEnabled)
        //{
        //    app.UseMultiTenancy();
        //}
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "基础接口");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "业务接口");

            // 模型的默认扩展深度，设置为 -1 完全隐藏模型
            c.DefaultModelsExpandDepth(1);
            // API文档仅展开标记
            c.DocExpansion(DocExpansion.List);
            c.DefaultModelRendering(ModelRendering.Example);
            c.DefaultModelExpandDepth(-1);
            // API前缀设置为空
            c.RoutePrefix = string.Empty;
            // API页面Title
            c.DocumentTitle = "😍接口文档 - 阿星Plus⭐⭐⭐";
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
