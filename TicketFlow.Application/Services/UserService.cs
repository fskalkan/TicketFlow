using AutoMapper;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Application.Interfaces.Services;

namespace TicketFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User not found.");

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateCurrentUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(updateUserDto.FullName))
                user.FullName = updateUserDto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
            {
                var email = updateUserDto.Email.Trim().ToLower();

                if (email != user.Email)
                {
                    var emailExists = await _userRepository.EmailExistsAsync(email);

                    if (emailExists)
                        throw new BadRequestException("Email address is already in use.");

                    user.Email = email;
                }
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);

            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserResponseDto>(user);
        }
    }
}