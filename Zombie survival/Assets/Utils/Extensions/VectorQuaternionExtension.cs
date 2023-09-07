using System.Text;
using UnityEngine;

namespace Assets.Utils.Extensions
{
    public static class VectorQuaternionExtension
    {
        public static string toJson(this Vector3 p, Quaternion? r)
        {
            StringBuilder pos = new();
            pos.Append("{");
            pos.Append($"\"PX\":\"{p.x.toFixed()}\",");
            pos.Append($"\"PY\":\"{p.y.toFixed()}\",");
            pos.Append($"\"PZ\":\"{p.x.toFixed()}\"");
            if (r != null)
            {
                pos.Append($",\"RX\":\"{r?.x.toFixed()}\",");
                pos.Append($"\"RY\":\"{r?.y.toFixed()}\",");
                pos.Append($"\"RZ\":\"{r?.z.toFixed()}\"");
            }
            pos.Append("}");

            return pos.ToString();
        }
    }
}
