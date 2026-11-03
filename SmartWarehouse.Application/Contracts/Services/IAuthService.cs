using SmartWarehouse.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto);
    }
}
