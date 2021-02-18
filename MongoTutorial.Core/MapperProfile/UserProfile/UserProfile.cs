using AutoMapper;
using MongoTutorial.Core.DTO.Users;
using MongoTutorial.Domain;

namespace MongoTutorial.Core.MapperProfile.UserProfile
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(u => u.Id, opt
                    => opt.MapFrom(u => u.Id))
                .ForMember(u => u.FullName, opt
                    => opt.MapFrom(u => u.FullName))
                .ForMember(u => u.UserName, opt
                    => opt.MapFrom(u => u.UserName))
                .ForMember(u => u.Email, opt
                    => opt.MapFrom(u => u.Email))
                .ForMember(u => u.PasswordHash, opt
                    => opt.MapFrom(u => u.PasswordHash))
                .ForMember(u => u.Phone, opt
                    => opt.MapFrom(u => u.Phone))
                .ForMember(u => u.RegistrationDateTime, opt
                    => opt.MapFrom(u => u.RegistrationDateTime))
                .ForMember(u => u.Roles, opt
                    => opt.MapFrom(u => u.Roles));

            CreateMap<User, UserDto>()
                .ForMember(u => u.Id, opt
                    => opt.MapFrom(u => u.Id))
                .ForMember(u => u.FullName, opt
                    => opt.MapFrom(u => u.FullName))
                .ForMember(u => u.UserName, opt
                    => opt.MapFrom(u => u.UserName))
                .ForMember(u => u.Email, opt
                    => opt.MapFrom(u => u.Email))
                .ForMember(u => u.PasswordHash, opt
                    => opt.MapFrom(u => u.PasswordHash))
                .ForMember(u => u.Phone, opt
                    => opt.MapFrom(u => u.Phone))
                .ForMember(u => u.RegistrationDateTime, opt
                    => opt.MapFrom(u => u.RegistrationDateTime))
                .ForMember(u => u.Roles, opt
                    => opt.MapFrom(u => u.Roles));

            CreateMap<UserModelDto, UserDto>()
                .ForMember(u => u.FullName, opt
                    => opt.MapFrom(u => u.FullName))
                .ForMember(u => u.UserName, opt
                    => opt.MapFrom(u => u.UserName))
                .ForMember(u => u.Email, opt
                    => opt.MapFrom(u => u.Email))
                .ForMember(u => u.Phone, opt
                    => opt.MapFrom(u => u.Phone));
        }
    }
}