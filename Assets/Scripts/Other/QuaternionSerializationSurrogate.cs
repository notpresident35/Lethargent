using System.Runtime.Serialization;
using UnityEngine;

sealed class QuaternionSerializationSurrogate : ISerializationSurrogate {

    // Method called to serialize a Quaternion object
    public void GetObjectData (System.Object obj,
                              SerializationInfo info, StreamingContext context) {

        Quaternion v4 = (Quaternion) obj;
        info.AddValue ("x", v4.x);
        info.AddValue ("y", v4.y);
        info.AddValue ("z", v4.z);
        info.AddValue ("w", v4.w);
        //Debug.Log (v4);
    }

    // Method called to deserialize a Quaternion object
    public System.Object SetObjectData (System.Object obj,
                                       SerializationInfo info, StreamingContext context,
                                       ISurrogateSelector selector) {

        Quaternion v4 = (Quaternion) obj;
        v4.x = (float) info.GetValue ("x", typeof (float));
        v4.y = (float) info.GetValue ("y", typeof (float));
        v4.z = (float) info.GetValue ("z", typeof (float));
        v4.w = (float) info.GetValue ("w", typeof (float));
        obj = v4;
        return obj;   // Formatters ignore this return value //Seems to have been fixed!
    }
}