using Application.DTOs.Accounts;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Account
        CreateMap<Account, AccountDto>();
        CreateMap<CreateAccountDto, Account>();
        CreateMap<UpdateAccountDto, Account>();
    }
}
