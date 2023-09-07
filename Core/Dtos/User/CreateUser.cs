using Server.Dtos.Position;
using Server.Entities;

namespace Core.Dtos.User
{

    public class CreateUserDto
    {
        public string SocketId { get; set; }
        public string UserName { get; set; }
        public CreatePositionDto LasPosition { get; set; }
    }
    public static class CreateUser
    {
        public static UserEntity toDto(this CreateUserDto m)
        {
            return new UserEntity
            {
                LasPosition = m.LasPosition.toDto(),
                UserName = m.UserName,
                SocketId = m.SocketId
            };


        }
    }
}
