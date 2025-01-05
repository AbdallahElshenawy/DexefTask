using AutoMapper;
using DexefTask.BusinessLogic.DTO.Book;
using DexefTask.DataAccess.Models;

namespace DexefTask.BusinessLogic.Services.AutoMapper
{
    public class BookProfile:Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookToReturnDTO>().ReverseMap();
            CreateMap<BookToAddDTO, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
