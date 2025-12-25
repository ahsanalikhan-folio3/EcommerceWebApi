using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.AdminProfiles;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class AdminProfileService : IAdminProfileService
    {
        private readonly IAuthRepository authRepository;
        private readonly IAdminProfileRepository adminProfileRepository;
        private readonly IMapper mapper;
        public AdminProfileService(IAuthRepository authRepository, IAdminProfileRepository adminProfileRepository, IMapper mapper)
        {
            this.authRepository = authRepository;
            this.adminProfileRepository = adminProfileRepository;
            this.mapper = mapper;
        }
        public async Task<bool> AddAdminProfile(AdminProfileDto adminProfile)
        {
            bool userAlreadyExist = await authRepository.UserExistByIdAsync(adminProfile.UserId);
            bool adminProfileAlreadyExist = await adminProfileRepository.AdminExistAsync(adminProfile.UserId);

            if (userAlreadyExist || adminProfileAlreadyExist) return false;

            var mappedProfile = mapper.Map<AdminProfile>(adminProfile);
            await adminProfileRepository.AddAdminProfile(mappedProfile);
            
            return true;
        }
    }
}
