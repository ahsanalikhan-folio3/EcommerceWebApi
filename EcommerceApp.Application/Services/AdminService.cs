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
            var result = await uow.Admins.ActivateUserAsync(userId);
            if (!result) return result;
            await uow.SaveChangesAsync();
            return result;
        }

        //public async Task<bool> AddAdminProfile(AdminProfileDto adminProfile)
        //{
        //    bool userAlreadyExist = await uow.Auth.UserExistByIdAsync(adminProfile.UserId);
        //    bool adminProfileAlreadyExist = await uow.Admins.AdminExistAsync(adminProfile.UserId);

        //    // If user does not exist or admin profile already exists, return false
        //    if (!userAlreadyExist || adminProfileAlreadyExist) return false;

        //    var mappedProfile = mapper.Map<AdminProfile>(adminProfile);
        //    await uow.Admins.AddAdminProfile(mappedProfile);
        //    await uow.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> DeActivateUser(Guid userId)
        {
            var result = await uow.Admins.DeActivateUserAsync(userId);
            if (!result) return result;
            await uow.SaveChangesAsync();
            return result;
        }
    }
}
