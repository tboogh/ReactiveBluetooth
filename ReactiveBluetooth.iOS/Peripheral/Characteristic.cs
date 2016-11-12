using System;
using System.Linq;
using System.Reactive.Linq;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using ReactiveBluetooth.iOS.Extensions;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property, PeripheralManagerDelegate.PeripheralManagerDelegate peripheralDelegate)
        {
            var nativePermissions = permission.ToCBAttributePermission();
            var nativeProperties = property.ToCBCharacteristicProperty();

            NSData nsData = null;
            if (value != null)
            {
                nsData = NSData.FromArray(value);
            }

            CBMutableCharacteristic mutableCharacteristic = new CBMutableCharacteristic(CBUUID.FromString(uuid.ToString()), nativeProperties, nsData, nativePermissions);
            NativeCharacteristic = mutableCharacteristic;

            ReadRequestObservable = peripheralDelegate.ReadRequestReceivedSubject.Where(x => x.AttRequest.Characteristic.UUID == NativeCharacteristic.UUID).Select(x => new AttRequest(x.AttRequest)).AsObservable();
            WriteRequestObservable = peripheralDelegate.WriteRequestsReceivedSubject.SelectMany(received => received.Requests.Where(y => y.Characteristic.UUID.Uuid.ToGuid() == Uuid).Select(x => new AttRequest(x)).ToObservable()).AsObservable();
            Subscribed = peripheralDelegate.CharacteristicSubscribedSubject.Where(x => x.Characteristic.UUID == NativeCharacteristic.UUID).Select(x => new Device(x.Central)).AsObservable();
            Unsubscribed = peripheralDelegate.CharacteristicUnsubscribedSubject.Where(x => x.Characteristic.UUID == NativeCharacteristic.UUID).Select(x => new Device(x.Central)).AsObservable();
        }

        public Guid Uuid => Guid.Parse(NativeCharacteristic.UUID.ToString());
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty();

        public IDescriptor[] Descriptors => NativeCharacteristic.Descriptors.Select(x => new Descriptor(x))
           .Cast<IDescriptor>()
           .ToArray();

        public CharacteristicPermission Permissions => NativeCharacteristic.Permissions.ToCharacteristicPermission();
        public CBMutableCharacteristic NativeCharacteristic { get; }
        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }
        public IObservable<IDevice> Subscribed { get; }
        public IObservable<IDevice> Unsubscribed { get; }
		public byte[] Value
		{
			get
			{
				return NativeCharacteristic.Value.ToArray();
			}
			set
			{
				NativeCharacteristic.Value = NSData.FromArray(value);
			}
		}
        public void AddDescriptor(IDescriptor descriptor)
        {
            var nativeDescriptor = ((Descriptor)descriptor).NativeDescriptor;
            var descriptors = NativeCharacteristic.Descriptors.ToList();
            descriptors.Add(nativeDescriptor);
            NativeCharacteristic.Descriptors = descriptors.ToArray();
        }
    }
}