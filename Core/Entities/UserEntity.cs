using Server.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Server.Entities
{
    public class UserEntity : EntityBase
    {
        public string SocketId { get; set; }
        public string UserName { get; set; }
        public string LastIP { get; set; }
        public Guid LasPositionId { get; set; }
        [ForeignKey("LasPositionId")]
        public virtual PositionEntity LasPosition { get; set; }

        public string toJson()
        {
            StringBuilder result = new StringBuilder();
            result.Append("{");
            result.Append($"\"{nameof(SocketId)}\":\"{SocketId}\",");
            result.Append($"\"{nameof(UserName)}\":\"{UserName}\",");
            result.Append($"\"{nameof(LasPositionId)}\":\"{LasPositionId}\",");
            result.Append($"\"{nameof(LasPosition)}\":{LasPosition.toJson()}");
            result.Append("}");


            return result.ToString();
        }
    }
}
