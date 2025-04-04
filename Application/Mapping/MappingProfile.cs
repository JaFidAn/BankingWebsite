using Application.DTOs.Accounts;
using Application.DTOs.Transactions;
using Application.DTOs.Payments;
using Application.DTOs.AuditLogs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ✅ Account
        CreateMap<Account, AccountDto>();
        CreateMap<CreateAccountDto, Account>();
        CreateMap<UpdateAccountDto, Account>();

        // ✅ Transaction
        CreateMap<CreateTransactionDto, Transaction>();
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // ✅ Payment
        CreateMap<CreatePaymentDto, Payment>();
        CreateMap<Payment, PaymentDto>();

        // ✅ AuditLog
        CreateMap<AuditLog, AuditLogDto>();
    }
}
