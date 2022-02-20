# TinyCLR.BitmapChart
This is simple chart for TinyCLR

![Chart 1](https://storagemurahaje.blob.core.windows.net/github/chart1.jpeg)

![Chart 2](https://storagemurahaje.blob.core.windows.net/github/chart2.jpeg)

## How to use

Just add the library to your project and here is the snippet:

```
var datas = new ArrayList();
            Random random = new Random();
            for(int i = 0; i < 10; i++)
            {
                datas.Add(new DataItem() { Value = random.Next(100), Name = $"N{i}" });
            }
            var chartService = new ChartService(800, 480)
            {
                DivisionAxisX = 1,
                DivisionAxisY = 15,
                RadiusPoint = 15,
                ChartTitle = "Test Tiny Chart",
                Items = datas
            };

            var chart1 = chartService.GetChart(ChartMode.LineMode);

            var chart2 = chartService.GetChart(ChartMode.RectangleMode);
            //.Save(Directory.GetCurrentDirectory() + "\\chartRectangle.png");
            GpioPin backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);
            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);

            var displayController = DisplayController.GetDefault();

            // Enter the proper display configurations
            displayController.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 800,
                Height = 480,
                DataFormat = DisplayDataFormat.Rgb565,
                Orientation = DisplayOrientation.Degrees0, //Rotate display.
                PixelClockRate = 24000000,
                PixelPolarity = false,
                DataEnablePolarity = false,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 16,
                HorizontalBackPorch = 46,
                HorizontalSyncPulseWidth = 1,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 7,
                VerticalBackPorch = 23,
                VerticalSyncPulseWidth = 1,
                VerticalSyncPolarity = false,
            });

            displayController.Enable(); //This line turns on the display I/O and starts
                                        //  refreshing the display. Native displays are
                                        //  continually refreshed automatically after this
                                        //  command is executed.

            
            var screen = Graphics.FromHdc(displayController.Hdc);

            screen.Clear();
            //draw first chart - line
            screen.DrawImage(chart1, 0, 0);

            screen.Flush();

            Thread.Sleep(5000);

            screen.Clear();
            //draw second chart - bar
            screen.DrawImage(chart2, 0, 0);

            screen.Flush();
           
            Thread.Sleep(-1);
```


Cheers :D.
