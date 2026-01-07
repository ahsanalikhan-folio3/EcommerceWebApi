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
        public async Task<bool> ActivateUser(int userId)
        {
            var result = await uow.Admins.ActivateUserAsync(userId);
            if (!result) return result;
            await uow.SaveChangesAsync();
            return result;
        }
        public async Task<bool> DeActivateUser(int userId)
        {
            var result = await uow.Admins.DeActivateUserAsync(userId);
            if (!result) return result;
            await uow.SaveChangesAsync();
            return result;
        }
        public async Task<bool> UpdateSellerOrderStatus (UpdateSellerOrderStatusFromAdminSideDto updateSellerOrderStatusFromAdminSide)
        {
            var result = await uow.SellerOrders.UpdateSellerOrderStatus(updateSellerOrderStatusFromAdminSide.SellerOrderId, updateSellerOrderStatusFromAdminSide.Status);
            await uow.SaveChangesAsync();
            return result;
        }
        public async Task<List<GetSellerOrderDto>> GetAllSellerOrders ()
        {
            var allSellerOrders = await uow.SellerOrders.GetAllSellerOrders();
            return mapper.Map<List<GetSellerOrderDto>>(allSellerOrders);
        }
        public async Task<List<GetSellerOrderDto>> GetAllSellerOrdersByStatus(OrderStatus status)
        {
            var allSellerOrders = await this.GetAllSellerOrders();
            return allSellerOrders.Where(so => so.Status == status).ToList();
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

    }
}
