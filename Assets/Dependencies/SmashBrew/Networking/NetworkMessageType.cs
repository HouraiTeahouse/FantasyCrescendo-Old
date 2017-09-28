using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public static class SmashNetworkMessages {
        public const short UpdatePlayer = MsgType.Highest + 1;
        public const short PlayerInput = MsgType.Highest + 1;
    }
    
}
