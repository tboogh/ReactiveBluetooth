using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Core.Peripheral
{
    public interface IBluetoothAbstractFactory
    {
        Central.IService CreateService();
        ICharacteristic CreateCharacteristic();
    }
}
