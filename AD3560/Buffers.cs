using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace SPIController
{
    static class Constants
    {
        public const byte ad5360_Mode = 0xc0;
        public const byte ad5360_A5 = 0x00;
        public const byte ad5360_Group1 = 0x08;
        public const byte ad5360_Group2 = 0x10;
        public const byte ad5360_Creg = 0x80;
        public const byte ad5360_Mreg = 0x40;
        public const byte ad5360_Read = 0x05; //00000101
        public const byte ad5360_BaseRegC = 0x44; //0100 0100
        public const byte ad5360_BaseRegM = 0x64; //0110 0100
        public const byte ad5360_LastBit = 0x40; //1000000
    }

    public enum Mode
    {
        ad5360_WriteCreg,
        ad5360_WriteMreg,
        ad5360_ReadCreg,
        ad5360_ReadMreg,
        ad5360_WriteBuff
    }

    public class _writeBuffer
    {
        public byte[] buffer = new byte[3];

        public _writeBuffer(int channel, float value, Mode mode = Mode.ad5360_WriteBuff)
        {
            switch(mode)
            {
                case Mode.ad5360_WriteBuff:
                    try
                    {
                        buffer[0] = BuildChannel(channel);
                        var voltValue = BuildVoltageValue(value);
                        buffer[1] = voltValue[0];
                        buffer[2] = voltValue[1];
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message.ToString();
                    }

                    break;

                case Mode.ad5360_WriteCreg:

                    buffer[0] = BuildCregChannel(channel);
                    var CalibCValue = BuildCalibrationValue(value);
                    buffer[1] = CalibCValue[0];
                    buffer[2] = CalibCValue[1];

                    break;

                case Mode.ad5360_WriteMreg:

                    buffer[0] = BuildMregChannel(channel);
                    var CalibMValue = BuildCalibrationValue(value);
                    buffer[1] = CalibMValue[0];
                    buffer[2] = CalibMValue[1];

                    break;
                case Mode.ad5360_ReadCreg:

                    buffer[0] = Constants.ad5360_Read;
                    var tmpReadCVal = BuildRegisterReadChannel(channel, Constants.ad5360_BaseRegC);
                    buffer[1] = tmpReadCVal[0];
                    buffer[2] = tmpReadCVal[1];

                    break;
                case Mode.ad5360_ReadMreg:
                    buffer[0] = Constants.ad5360_Read;
                    var tmpReadMVal = BuildRegisterReadChannel(channel, Constants.ad5360_BaseRegM);
                    buffer[1] = tmpReadMVal[0];
                    buffer[2] = tmpReadMVal[1];

                    break;
            }
        }

        private Byte BuildChannel(int channel)
        {
            Byte result;
            if (channel < 8)
            {
                Byte val = Convert.ToByte(channel);
                result = Constants.ad5360_Mode | Constants.ad5360_Group1;
                result += val;
            }
            else
            {
                Byte val = Convert.ToByte(channel - 8);
                result = Constants.ad5360_Mode | Constants.ad5360_Group2;
                result += val;

            }

            return result;
        }

        private byte BuildCregChannel(int channel)
        {
            Byte result;
            if (channel < 8)
            {
                Byte val = Convert.ToByte(channel);
                result = Constants.ad5360_Creg | Constants.ad5360_Group1;
                result += val;
            }
            else
            {
                Byte val = Convert.ToByte(channel - 8);
                result = Constants.ad5360_Creg | Constants.ad5360_Group2;
                result += val;

            }

            return result;

        }

        private byte[] BuildRegisterReadChannel(int channel, int baseValue)
        {

            Byte[] result = new byte[2];
            var value = baseValue + (channel / 2);
            if(channel%2 != 0)
            {
                result[1] = Constants.ad5360_LastBit;
            }
            result = BitConverter.GetBytes(value);

            return result;
        }

        private byte BuildMregChannel(int channel)
        {
            Byte result;
            if (channel < 8)
            {
                Byte val = Convert.ToByte(channel);
                result = Constants.ad5360_Mreg | Constants.ad5360_Group1;
                result += val;
            }
            else
            {
                Byte val = Convert.ToByte(channel - 8);
                result = Constants.ad5360_Mreg | Constants.ad5360_Group2;
                result += val;

            }

            return result;

        }

        private Byte[] BuildCalibrationValue(float value)
        {
            Byte[] result = new Byte[2];
            try
            {
                UInt16 iValue = Convert.ToUInt16(value);
                var bValue = BitConverter.GetBytes(iValue);
                result[0] = bValue[1];
                result[1] = bValue[0];
            }
            catch (Exception ex)
            {
                var tmp = ex.Message.ToString();
            }
            return result;
        }

        private Byte[] BuildVoltageValue(float volts)
        {
            Byte[] result = new Byte[2];
            var value = ((volts) * 65536);
            value = value / 10;
            value = value + 32768;
            try
            {
                UInt16 iValue = Convert.ToUInt16(value);
                var bValue = BitConverter.GetBytes(iValue);
                result[0] = bValue[1];
                result[1] = bValue[0];
            }
            catch (Exception ex)
            {
                var tmp = ex.Message.ToString();
            }
            return result;
        }
    }
    public class _TimeSlice
    {
        public List<_writeBuffer> timeSlice = new List<_writeBuffer>();
    }

}
