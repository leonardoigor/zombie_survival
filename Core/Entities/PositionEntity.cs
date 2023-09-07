using Server.Dtos.Position;
using Server.Entities.Base;
using System;
using System.Text;

namespace Server.Entities
{
    public class PositionEntity : EntityBase
    {

        public double PX { get; set; }
        public double PY { get; set; }
        public double PZ { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }

        internal object toJson()
        {
            StringBuilder r = new StringBuilder();
            r.Append("{");
            r.Append($"\"{nameof(PX)}\":\"{PX}\",");
            r.Append($"\"{nameof(PY)}\":\"{PY}\",");
            r.Append($"\"{nameof(PZ)}\":\"{PZ}\",");
            r.Append($"\"{nameof(RX)}\":\"{RX}\",");
            r.Append($"\"{nameof(RY)}\":\"{RY}\",");
            r.Append($"\"{nameof(RZ)}\":\"{RZ}\"");
            r.Append("}");

            return r.ToString();
        }

        public static explicit operator PositionEntity(UpdatePosition v)
        {
            double.TryParse(v.Rx, out double  RX);
            double.TryParse(v.Ry, out double  RY);
            double.TryParse(v.Rz, out double  RZ);   
            double.TryParse(v.Px, out double  PX);
            double.TryParse(v.Py, out double  PY);
            double.TryParse(v.Pz, out double  PZ);
            return new PositionEntity {
                Id = Guid.Empty,
                RX = RX,
                RY=  RY,
                RZ = RZ,     
                PX = PX, 
                PY=  PY,
                PZ = PZ,
            };
        }

    }
}
