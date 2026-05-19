using AutoMapper;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Application.Interfaces.Services;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var email = registerUserDto.Email.Trim().ToLower();

            var emailExists = await _userRepository.EmailExistsAsync(email);

            if (emailExists)
                throw new BadRequestException("Email address is already in use.");

            var user = new User
            {
                FullName = registerUserDto.FullName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password),
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto)
        {
            var email = loginUserDto.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
                throw new UnauthorizedException("Invalid email or password.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedException("User account is not active.");

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                User = _mapper.Map<UserResponseDto>(user)
            };
        }
    }
}