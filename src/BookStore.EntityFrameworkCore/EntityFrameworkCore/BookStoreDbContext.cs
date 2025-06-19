using BookStore.Authors;
using BookStore.Books;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace BookStore.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class BookStoreDbContext :
    AbpDbContext<BookStoreDbContext>
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();

        /* Configure your own tables/entities inside here */

        //配置实体
        builder.Entity<Book>(b =>
        {
            //把实体到数据库映射改个表名字BookStoreConsts.DbTablePrefix+"Book" 或者 nameof(Book)
            b.ToTable(BookStoreConsts.DbTablePrefix + nameof(Book), BookStoreConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props 自动配置属性
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);//配置必填最大长度
        });
        //作者
        builder.Entity<Author>(b =>
        {
            //把实体到数据库映射改个表名字  "Author" 或者 nameof(Author)
            b.ToTable(BookStoreConsts.DbTablePrefix + nameof(Author), BookStoreConsts.DbSchema);
            b.ConfigureByConvention(); // 自动配置属性
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);//配置必填最大长度
        });
    }
}
