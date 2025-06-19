using AutoMapper;
using BookStore.Books;

namespace BookStore;

public class BookStoreApplicationAutoMapperProfile : Profile
{
    public BookStoreApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Books.Book, Books.BookDto>()
            .ForMember(desk => desk.Id, opt => opt.MapFrom(src => src.Id)).ReverseMap();
        CreateMap<CreateUpdataBookDto, Book>();
    }
}
