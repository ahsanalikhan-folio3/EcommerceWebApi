using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        public AdminService(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        public async Task<bool> ActivateUser(Guid userId)
        {
            return await uow.Admins.ActivateUserAsync(userId);  
        }

        public async Task<bool> AddAdminProfile(AdminProfileDto adminProfile)
        {
            bool userAlreadyExist = await uow.Auth.UserExistByIdAsync(adminProfile.UserId);
            bool adminProfileAlreadyExist = await uow.Admins.AdminExistAsync(adminProfile.UserId);

            // If user does not exist or admin profile already exists, return false
            if (!userAlreadyExist || adminProfileAlreadyExist) return false;

            var mappedProfile = mapper.Map<AdminProfile>(adminProfile);
            await uow.Admins.AddAdminProfile(mappedProfile);
            
            return true;
        }

        public Task<bool> DeActivateUser(Guid userId)
        {
            return uow.Admins.DeActivateUserAsync(userId);
        }
    }
}
