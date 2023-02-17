/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */

using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.MessageTypes.Std
{
    public class Int8 : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "std_msgs/Int8";

        public sbyte data;

        public Int8()
        {
            this.data = 0;
        }

        public Int8(sbyte data)
        {
            this.data = data;
        }
    }
}
