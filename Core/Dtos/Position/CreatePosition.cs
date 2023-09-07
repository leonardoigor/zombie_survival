using Server.Entities;

namespace Server.Dtos.Position
{

    public class CreatePositionDto
    {
        public double PX { get; set; }
        public double PY { get; set; }
        public double PZ { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }
    }
    public static class CreatePosition
    {
        public static PositionEntity toDto(this CreatePositionDto m)
        {
            return new PositionEntity
            {
                PX = m.PX,
                PY = m.PY,
                PZ = m.PZ,
                RX = m.RX,
                RY = m.RY,
                RZ = m.RZ,
            };


        }
    }
}
