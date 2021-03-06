﻿using System;
using AutoMapper;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Core.MapperProfile.AuthProfile
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDto, UserDto>()
                .ForMember(u => u.Id, opt =>
                    opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(u => u.Email, opt =>
                    opt.MapFrom(r => r.Email))
                .ForMember(u => u.Phone, opt =>
                    opt.MapFrom(r => r.Phone))
                .ForMember(u => u.FullName, opt =>
                    opt.MapFrom(r => r.FullName))
                .ForMember(u => u.UserName, opt =>
                    opt.MapFrom(r => r.UserName))
                .ForMember(u => u.RegistrationDateTime, opt =>
                    opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(u => u.SessionId, opt =>
                    opt.MapFrom(_ => Guid.NewGuid().ToString()));
        }
    }
}