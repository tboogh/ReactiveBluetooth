using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using ReactiveBluetooth.Core.Peripheral;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class AttRequest : IAttRequest
    {
        public ICharacteristic Characteristic { get; }
        public int Offset => (int) CBAttRequest.Offset;
        public byte[] Value => CBAttRequest.Value.ToArray();
        public CBATTRequest CBAttRequest { get; }

        public AttRequest(ICharacteristic characteristic, CBATTRequest cbattRequest)
        {
            Characteristic = characteristic;
            CBAttRequest = cbattRequest;
        }
    }
}
