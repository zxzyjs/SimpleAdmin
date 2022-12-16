﻿using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 启动项配置
/// </summary>
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //启动系统设置ConfigureServices组件
        services.AddComponent<AppSettingsComponent>();
        //认证组件
        services.AddComponent<AuthComponent>();
        //gip压缩
        services.AddComponent<GzipCompressionComponent>();
        //事件总线
        services.AddEventBus();
        //添加控制器相关
        services.AddControllers()
                     .AddInjectWithUnifyResult<SimpleAdminResultProvider>()//配置统一返回模型
                     .AddNewtonsoftJson(options =>//配置json
                     {
                         options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // 首字母小写（驼峰样式）
                         options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss"; // 时间格式化                                          
                         options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // 忽略循环引用
                     });


    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //启动系统设置Configure组件
        app.UseComponent<AppSettingsApplicationComponent>(env);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            //启用gzip压缩
            app.UseResponseCompression();
        }
        // 启用HTTPS
        app.UseHttpsRedirection();

        // 添加状态码拦截中间件
        app.UseUnifyResultStatusCodes();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}