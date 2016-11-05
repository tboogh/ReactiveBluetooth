using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Android.Bluetooth;
using Java.Util;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using Observable = System.Reactive.Linq.Observable;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Characteristic : ICharacteristic, IDisposable
    {
        public BluetoothGattServer GattServer { get; set; }
        private readonly IDisposable _descriptorReadDisposable;

        public Characteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property, IServerCallback serverCallback)
        {
            var nativePermissions = permission.ToGattPermission();
            var nativeProperties = property.ToGattProperty();

            var characteristic = new BluetoothGattCharacteristic(UUID.FromString(uuid.ToString()), nativeProperties, nativePermissions);
            if (!characteristic.SetValue(value))
            {
                throw new Exception("Failed to set characteristic value");
            }
            NativeCharacteristic = characteristic;

            ReadRequestObservable = serverCallback.CharacteristicReadRequestSubject
                .Where(x => x.Item4.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Item3, null, request.Item2, request.Item1)).AsObservable(); ;

            WriteRequestObservable = serverCallback.CharacteristicWriteRequestSubject
                .Where(x => x.Item3.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Item6, request.Item7, request.Item2, request.Item1)).AsObservable(); ;

            _descriptorReadDisposable = serverCallback.DescriptorReadRequestSubject.Where(x => Guid.Parse(x.Item4.Uuid.ToString()) == "2902".ToGuid())
                .Subscribe(tuple =>
                {
                    GattServer.SendResponse(tuple.Item1, tuple.Item2, GattStatus.Success, 0, null);
                });

            Subscribed = serverCallback.DescriptorWriteRequestSubject.Where(x =>
            {
                var desc = x.Item3.Uuid.ToString()
                    .ToGuid() == "2902".ToGuid();
                var chara = x.Item3.Characteristic.Uuid == characteristic.Uuid;
                if (desc && chara)
                {
                    var dValue = x.Item3.GetValue();
                    
                }
                if (desc && chara)
                {
                    GattServer.SendResponse(x.Item1, x.Item2, GattStatus.Success, 0, null);
                }
                return desc && chara;
            })
                .Select(x => new Device(x.Item1)).AsObservable();

            Unsubscribed = Observable.Return<IDevice>(null);
        }

        public BluetoothGattCharacteristic NativeCharacteristic { get; }
        public Guid Uuid => Guid.Parse(NativeCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty();
        public IDescriptor[] Descriptors => NativeCharacteristic.Descriptors?.Select(x => new Descriptor(x))
           .Cast<IDescriptor>()
           .ToArray();

        public IObservable<IDevice> Unsubscribed { get; }
        public CharacteristicPermission Permissions => NativeCharacteristic.Permissions.ToCharacteristicPermission();
        public void AddDescriptor(IDescriptor descriptor)
        {
            var nativeDescriptor = ((Descriptor) descriptor).NativeDescriptor;
            NativeCharacteristic.AddDescriptor(nativeDescriptor);
        }

        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }
        public IObservable<IDevice> Subscribed { get; }

		public byte[] Value
		{
			get
			{
				return NativeCharacteristic.GetValue();
			}
			set
			{
				NativeCharacteristic.SetValue(value);
			}
		}

		public void Dispose()
        {
            _descriptorReadDisposable?.Dispose();
        }
    }
}