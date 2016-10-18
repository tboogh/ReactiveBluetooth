using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using ReactiveBluetooth.Core.Peripheral;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class AttRequest : IAttRequest
    {
        public int Offset => (int) CbAttRequest.Offset;
        public byte[] Value => CbAttRequest.Value.ToArray();
        public CBATTRequest CbAttRequest { get; }

        public AttRequest(CBATTRequest cbattRequest)
        {
            CbAttRequest = cbattRequest;
        }
    }
}
