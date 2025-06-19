using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace BookStore;

[DependsOn(
    typeof(BookStoreDomainModule),
    typeof(BookStoreApplicationContractsModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class BookStoreApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<BookStoreApplicationModule>();
        });
        //可以配置其他服务
    }
}
