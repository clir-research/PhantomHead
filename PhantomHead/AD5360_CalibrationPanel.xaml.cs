using SPIController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PhantomHead
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AD5360_CalibrationPanel : Page
    {
        private Boolean _StopEnabled = true;
        private const int ENABLE_PIN = 18;
        private GpioPin _enable_Timer;
        private float[] CalibrationValueCreg;
        private float[] CalibrationValueMreg;

        AD5360_Controller ad5360;

        public AD5360_CalibrationPanel()
        {
            this.InitializeComponent();
            ad5360 = new AD5360_Controller(SpiMode.Mode1, (200 * 1000), 0, "SPI0");
            CalibrationValueCreg = new float[16];
            CalibrationValueMreg = new float[16];
            Init_Enable();
        }

        private void Init_Enable()
        {
            GpioOpenStatus status = new GpioOpenStatus();
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                return;
            }

            var open = gpio.TryOpenPin(ENABLE_PIN, GpioSharingMode.Exclusive, out _enable_Timer, out status);
            if (open == true)
            {
                _enable_Timer.SetDriveMode(GpioPinDriveMode.Output);
                _enable_Timer.Write(GpioPinValue.Low);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Frame.Navigate(typeof(MainPage), mainPage);
        }

        private void WriteCalibrateChannel(int channel, float regC, float regM)
        {
            var RegCBuffer = new _writeBuffer((int)channel, regC, Mode.ad5360_WriteCreg);
            var RegMBuffer = new _writeBuffer((int)channel, regM, Mode.ad5360_WriteMreg);
            ad5360.WriteBuffer(RegCBuffer);
            ad5360.WriteBuffer(RegMBuffer);
        }



        #region Get Calibration Values

        private string GetCalibrationValues(int channel, SPIController.Mode mode)
        {
            _writeBuffer writeBuffer = new _writeBuffer(channel, 0, mode);

            var result = ad5360.ReadBuffer(writeBuffer);

            var tmp = BitConverter.ToInt32(result, 0);

            return tmp.ToString();

        }

        private void GetCalibValues_click(object sender, RoutedEventArgs e)
        {
            if (ad5360.active)
            {
                //GetCurentCalibrationValues(0);
                throw new NotImplementedException();
            }
        }

        private void ChannelCalibrate_Click(object sender, RoutedEventArgs e)
        {
            var channel = Convert.ToInt32(txt_Channel.Text);
            var cReg = Convert.ToSingle(txt_ChannelCRegister.Text);
            var mReg = Convert.ToSingle(txt_ChannelMRegister.Text);
            if (channel > 15)
            {
                txt_ChannelCRegister.Text = "-1";
                txt_ChannelMRegister.Text = "-1";
                return;
            }
            if (ad5360.active)
            {
                if (Ch0WriteChkBox.IsChecked == true)
                {
                    //write values
                    WriteCalibrateChannel(channel, cReg, mReg);
                    //read new values
                    txt_ChannelCRegister.Text = GetCalibrationValues(channel, Mode.ad5360_ReadCreg);
                    txt_ChannelMRegister.Text = GetCalibrationValues(channel, Mode.ad5360_ReadMreg);
                }
                else
                {
                    txt_ChannelCRegister.Text = GetCalibrationValues(channel, Mode.ad5360_ReadCreg);
                    txt_ChannelMRegister.Text = GetCalibrationValues(channel, Mode.ad5360_ReadMreg);
                }
            }
        }

        private void Channel1Calibrate_Click(object sender, RoutedEventArgs e)
        {
            if (ad5360.active)
            {
                if (Ch0WriteChkBox.IsChecked == true)
                {
                    //write values
                    WriteCalibrateChannel(0, Convert.ToSingle(txt_ChannelCRegister.Text), Convert.ToSingle(txt_ChannelMRegister.Text));
                }
                else
                {
                    txt_Channel1CRegister.Text = GetCalibrationValues(1, Mode.ad5360_ReadCreg);
                    txt_Channel1MRegister.Text = GetCalibrationValues(1, Mode.ad5360_ReadMreg);
                }
            }
        }

        private void Channel2Calibrate_Click(object sender, RoutedEventArgs e)
        {
            if (ad5360.active)
            {
                if (Ch0WriteChkBox.IsChecked == true)
                {
                    //write values
                    WriteCalibrateChannel(0, Convert.ToSingle(txt_Channel2CRegister.Text), Convert.ToSingle(txt_Channel2MRegister.Text));
                }
                else
                {
                    txt_Channel2CRegister.Text = GetCalibrationValues(2, Mode.ad5360_ReadCreg);
                    txt_Channel2MRegister.Text = GetCalibrationValues(2, Mode.ad5360_ReadMreg);
                }
            }
        }

        private void Channel3Calibrate_Click(object sender, RoutedEventArgs e)
        {
            if (ad5360.active)
            {
                if (Ch0WriteChkBox.IsChecked == true)
                {
                    //write values
                    WriteCalibrateChannel(0, Convert.ToSingle(txt_Channel3CRegister.Text), Convert.ToSingle(txt_Channel3MRegister.Text));
                }
                else
                {
                    txt_Channel3CRegister.Text = GetCalibrationValues(3, Mode.ad5360_ReadCreg);
                    txt_Channel3MRegister.Text = GetCalibrationValues(3, Mode.ad5360_ReadMreg);
                }
            }
        }


        #endregion
    }
}
