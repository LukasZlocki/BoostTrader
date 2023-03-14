using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BoostTrader2._0.IOAdapters;
using BoostTrader2._0.ChartsLib;
using BoostTrader2._0.Grid;
using BoostTrader2._0.UserSetup;



namespace BoostTrader2._0
{

    /// <summary>
    /// BoostTrader by 3Dev 
    /// Ver 1.0
    /// Programmed : 2017 - 2018
    /// 
    /// what's new: - InProgress Version
    /// [06062018] -> auto rescaling values on chart
    /// [30032018] -> Labels added to chart screen
    /// </summary>


    public partial class MainWindow : Window
    {
        // for selecting proper data in IO Adapters
        private static string POLISH_STOCKS = "GPW";
        private static string POLISH_FUNDS = "TFI";

        private static int MAX_USAGE_RANGE = 740; // dif additional button added number need to be increased
        private static int LABELS_QUANTITY = 15;

        private int GlobalOldUserRange = 0;
        private int GlobalUserRange = 0;

        // name of stock selected by user
        private string UserStock = "WIELTON";

        // 
        private bool GpwButtonOn = true;
        private bool TfiButtonOn = false;


        #region Object to draw charts and grids
        // Object to draw charts
        Rectangle[] shadow = new Rectangle[MAX_USAGE_RANGE];
        Rectangle[] candle = new Rectangle[MAX_USAGE_RANGE];

        // Objectd to draw grid
        Label[] lblStepValue = new Label[LABELS_QUANTITY];
        Line[] grideLine = new Line[MAX_USAGE_RANGE];
        #endregion


        // Data from file list
        List<DataCollector> dataCollectorList = new List<DataCollector>();

        // Charts Library objects
        DataCollector[] dataCollector = new DataCollector[MAX_USAGE_RANGE];


        AxisCalibration axisCalib = new AxisCalibration();
        GridCalibration gridCalib;
        PriceEvaluation priceEvaluation;
        DateEvaluation dateEvaluation;

        CandleChart candlePropperties = new CandleChart();


        #region UserSetups

        UserPreferencesScreenColours userPreferencesScreenColours;

        #endregion




        public MainWindow()
        {
            InitializeComponent();
        }

        // first data after loading window
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 240;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }


        #region Main Looop of the program

        private void MainLoop(ref int oldUserRange, ref int userRange, string stockName)
        {

            #region Removing objects from screen - clearing screen
            RemoveChartFromScreen(oldUserRange);
            RemoveGridAndLabelsFromScreen(LABELS_QUANTITY);
            #endregion


            #region Object initialisation for data collecting class
            DataCollectorObjectInit(userRange);
            #endregion


            #region Loading data from database
            LoadDataFromDataBase(ref dataCollector, userRange, stockName);
            #endregion


            #region Set up screen - Calibration of axis
            axisCalib.SetScreenSize(this.canvasChartScreen.ActualWidth, this.canvasChartScreen.ActualHeight);
            axisCalib.SetData(dataCollector, GlobalUserRange);  // transfering data for calculating Axises     
            #endregion


            #region Drawing candles
            StartDrawingProcedure();
            #endregion


            #region Price and Date evaluation
            // Price evaluation set up for vertical label
            priceEvaluation = new PriceEvaluation(axisCalib.GetMaxValueY(), axisCalib.GetMinValueY(), axisCalib.GetScreenSizeY());

            // Date evaluation set up for horizontal label
            dateEvaluation = new DateEvaluation(userRange, axisCalib.GetScreenSizeX(), dataCollector);
            #endregion


            #region calibration of grid
            gridCalib = new GridCalibration(axisCalib.GetMinValueY(), axisCalib.GetMaxValueY());
            oldUserRange = userRange;
            #endregion


            #region Drawing Labels & chatch
            DrawLabels(axisCalib.GetScreenSizeX(), axisCalib.GetScreenSizeY(), gridCalib.GetQuantityOfStepsYAxis(), gridCalib.GetUnitsPerStepYAxis());
            DrawChatchOnGrid(axisCalib.GetScreenSizeX(), axisCalib.GetScreenSizeY(), gridCalib.GetQuantityOfStepsYAxis(), gridCalib.GetUnitsPerStepYAxis());
            #endregion


            #region Delivery on screeen importand data for programmer
            lbl1.Content = "Screen size Y = " + axisCalib.GetScreenSizeY();
            lbl2.Content = "Max value Y = " + axisCalib.GetMaxValueY();
            lbl3.Content = "Min Price : " + axisCalib.GetMinValueY();
            lbl4.Content = "Max Price : " + axisCalib.GetMaxValueY();
            #endregion


        }

        #endregion


        # region Mouse Tracking

        // Mouse tracking method
        private void canvasGridScreen_MouseMove(object sender, MouseEventArgs e)
        {
            double _mX, _mY; // possition of Mouse in grid
            double _valueY; // price in specific px point 
            string _valueX; // date in specific px point

            // possition of mouse in grid
            _mX = Mouse.GetPosition(this).X;
            _mY = canvasGridScreen.ActualHeight - Mouse.GetPosition(canvasGridScreen).Y; // reverse riding of mouse possition that is why I need to substract

            // Price value evaluation to print on screan basis on pixels
            _valueY = priceEvaluation.GetPriceOnThisPixel(_mY);

            // Price value evaluation to print on screan basis on pixels
            _valueX = dateEvaluation.GetDateByPossition(_mX);

            // lbls update
            lbl5.Content = " Mouse Y : " + _mY;
            lbl6.Content = " Value Y : " + _valueY;
            lblPrice.Content = String.Format("{0:0.00}", _valueY);
            lblDate.Content = String.Format("{0}", _valueX);

            // moving user label with price
            DrawingAndMovingUserLabelWithPrice(Mouse.GetPosition(canvasGridScreen).X, Mouse.GetPosition(canvasGridScreen).Y);

            // Drawing and moving user line with price
            DrawingUserLine(Mouse.GetPosition(canvasGridScreen).Y, Mouse.GetPosition(canvasGridScreen).X);
        }

        private void DrawingAndMovingUserLabelWithPrice(double mousePosX, double mousePosY)
        {
            int _VERTICAL_LABEL_TO_MIDDLE = 12; // set up user label in middle of user line
            int _HORIZONTAL_LABEL_TO_MIDDLE = 30; // set up user label in middle of user line

            // Vertical moving label section - label with price 
            Canvas.SetTop(lblPrice, mousePosY - _VERTICAL_LABEL_TO_MIDDLE);
            Canvas.SetLeft(lblPrice, 0);


            // Horizontal moving label section - label with date   
            Canvas.SetTop(lblDate, 0);
            Canvas.SetLeft(lblDate, mousePosX - _HORIZONTAL_LABEL_TO_MIDDLE);
        }

        private void DrawingUserLine(double mousePosY, double mousePosX)
        {
            // Horizontal line section
            UserHorizontalLine.X1 = 0;
            UserHorizontalLine.Y1 = 0;
            UserHorizontalLine.X2 = canvasChartScreen.ActualWidth;
            UserHorizontalLine.Y2 = 0;
            Canvas.SetTop(UserHorizontalLine, mousePosY);
            Canvas.SetLeft(UserHorizontalLine, 0);


            // Horizontal line section
            UserVerticalLine.X1 = mousePosX;
            UserVerticalLine.Y1 = canvasChartScreen.ActualHeight;
            UserVerticalLine.X2 = mousePosX;
            UserVerticalLine.Y2 = -canvasChartScreen.ActualHeight; ;
            Canvas.SetTop(UserVerticalLine, mousePosY);
            Canvas.SetLeft(UserVerticalLine, 0);

        }

        #endregion

        #region Buttons

        #region Buttons - Market Choose (TFI, GPW, ...)

        private void btnGPWChoice_Click(object sender, RoutedEventArgs e)
        {
            GpwButtonOn = true;
            TfiButtonOn = false;

            btnGPWChoice.Background = Brushes.White;
            btnTFIChoice.Background = Brushes.Gray;
        }

        private void btnTFIChoice_Click(object sender, RoutedEventArgs e)
        {
            GpwButtonOn = false;
            TfiButtonOn = true;
          
            btnGPWChoice.Background = Brushes.Gray;
            btnTFIChoice.Background = Brushes.White;
        }
        #endregion

        #region Buttons - UserScreen setup

        private void btnbtnUserColorPreferences_Click(object sender, RoutedEventArgs e)
        {
            Point position = btnUserColorPreferences.PointToScreen(new Point(0d, 0d)),
            controlPosition = this.PointToScreen(new Point(0d, 0d));

            position.X -= controlPosition.X;
            position.Y -= controlPosition.Y;

            double _windowStartingPoint = position.X;

            ColorPreferences colorPreferences = new ColorPreferences();
            colorPreferences.Left = _windowStartingPoint;
            colorPreferences.Top = 30;
            colorPreferences.Show();

        }

        private void btnBackGroundWhite_Click(object sender, RoutedEventArgs e)
        {
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
            canvasChartScreen.Background = new SolidColorBrush(Colors.White);
            UserHorizontalLine.Stroke = new SolidColorBrush(Colors.Black);
            UserVerticalLine.Stroke = new SolidColorBrush(Colors.Black);
        }

        private void btnBackGroundGray_Click(object sender, RoutedEventArgs e)
        {
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
            canvasChartScreen.Background = new SolidColorBrush(Colors.Gray);
            UserHorizontalLine.Stroke = new SolidColorBrush(Colors.White);
            UserVerticalLine.Stroke = new SolidColorBrush(Colors.White);
        }

        #endregion

        #region Buttons - time range

        private void btn1m_Click(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 20;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }

        private void btn3m_Click(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 60;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }

        private void btn6m_Click(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 120;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }

        private void btn1y_Click(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 240;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }

        private void btn2y_Click(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 480;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }

        private void btn3y_Click(object sender, RoutedEventArgs e)
        {
            GlobalUserRange = 720;
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, UserStock);
        }

        #endregion 

        #region Buttons - Stock Choice

        private void btnStocksChoice_Click(object sender, RoutedEventArgs e)
        {
            string _marketChoosedByUser = "";
          
            Point position = btnStocksChoice.PointToScreen(new Point(0d, 0d)),
            controlPosition = this.PointToScreen(new Point(0d, 0d));

            position.X -= controlPosition.X;
            position.Y -= controlPosition.Y;

            double _windowStartingPoint = position.X;

            // checking which market was choosen by user
            _marketChoosedByUser = GiveMeChoosedMarket();

            StockChoice stockChoiceWindow = new StockChoice(this, _marketChoosedByUser);
            stockChoiceWindow.Left = _windowStartingPoint;
            stockChoiceWindow.Top = 30;
            stockChoiceWindow.Show();

        }

        #endregion

        // Closing main window
        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion


        #region Start drawing procedure
        private void StartDrawingProcedure()
        {
                   
            /*
            // calibration of grid
            gridCalib = new GridCalibration(axisCalib.GetMinValueY(), axisCalib.GetMaxValueY());
            */

            // setting candle properties
            candlePropperties.CandleWidthCalculation(axisCalib.GetProbeQuantityX(), axisCalib.GetScreenSizeX());
   
            // draw data chart on screen
            RefreshScreen(dataCollector, GlobalUserRange);
        }
        
        #endregion


        #region Removing chart / grid from screen
        private void RemoveChartFromScreen (int userRange)
        {
            for (int i = 0; i < GlobalOldUserRange; i++)
            {
                canvasChartScreen.Children.Remove(shadow[i]);
                canvasChartScreen.Children.Remove(candle[i]);
            }
        }

        private void RemoveGridAndLabelsFromScreen(int lablesQuantity)
        {
            for (int i = 0; i < lablesQuantity; i++)
            {
                canvasrightMenu.Children.Remove(lblStepValue[i]);
                canvasChartScreen.Children.Remove(grideLine[i]);
            }
        }
        #endregion 


        #region Drawing scale chatch procedure
        // Drawing of hatch on chart along with data value on X and Y Axis
        private void DrawChatchOnGrid(double screenSizeX, double screeenSizeY, int gridSteps, int gridUnitsPerStep)
        {
            int _unitsPerStep = 0;
            double _labelValue = 0;
            double _yPos = 0;

            for (int i = 0; i < gridSteps + 1 ; i++)
            {
                _labelValue = gridCalib.GetLabelValue(i);
                _yPos = axisCalib.GetYaxisPixelPosByPrice(_labelValue);


                grideLine[i] = new Line();

                grideLine[i].Stroke = System.Windows.Media.Brushes.White;
                grideLine[i].Fill = System.Windows.Media.Brushes.White;
                grideLine[i].StrokeThickness = 0.5;

                grideLine[i].X1 = 0;
                grideLine[i].Y1 = 0;

                grideLine[i].X2 = canvasChartScreen.ActualWidth;
                grideLine[i].Y2 = 0;

                canvasChartScreen.Children.Add(grideLine[i]);
               // Canvas.SetBottom(grideLine[i],  _unitsPerStep);
                // Canvas.SetBottom(grideLine[i], _unitsPerStep * axisCalib.GetPixelUnitY());    /<- old version Delete it ! 
                Canvas.SetBottom(grideLine[i], _yPos);
                Canvas.SetLeft(grideLine[i], 0);

                _unitsPerStep = _unitsPerStep + gridUnitsPerStep;
            }
        }  
        #endregion


        #region Drawing labels with value
        // Drawing GRID labels according to calculated steps 
        private void DrawLabels(double screenSizeX, double screeenSizeY, int gridSteps, int gridUnitsPerStep)
        { 
          int _unitsPerStep = 0;
          double _labelValue = 0;
          double _yPos = 0;
    
            for (int i = 0; i < gridSteps + 1; i++)
          {
              _labelValue = gridCalib.GetLabelValue(i);
              _yPos = axisCalib.GetYaxisPixelPosByPrice(gridCalib.GetLabelValue(i));

              lblStepValue[i] = new Label();
              
              lblStepValue[i].Height = 30;
              lblStepValue[i].FontSize = 10;
              lblStepValue[i].Foreground = System.Windows.Media.Brushes.White;
              lblStepValue[i].Content = "- " + _labelValue;

              canvasrightMenu.Children.Add(lblStepValue[i]);
             // Canvas.SetBottom(lblStepValue[i], _unitsPerStep * axisCalib.GetPixelUnitY()-15); // -15px middle label height     /<- old version Delete it ! 
              Canvas.SetBottom(lblStepValue[i], _yPos - 15); // -15px middle label height
              Canvas.SetLeft(lblStepValue[i],0);

              _unitsPerStep = _unitsPerStep + gridUnitsPerStep;
          }
        }

        #endregion


        #region Drawing Candle Procedure

        // showing candle by candle according to probe quantity
        private void RefreshScreen(DataCollector[] data, int probes)
        {
            for (int i = 0; i < probes; i++)
            {
                showCandle(data[i].GetStartPrice(), data[i].GetEndPrice(), data[i].GetMinPrice(), data[i].GetMaxPrice(), i);
            }

        }

        private void showCandle(double startPrice, double endPrice, double lowestPrice, double highestPrice, int dayNb)
        {

            //   MessageBox.Show("minValue: " + axisCalib.GetMinValueY() + " Transformacja: " + axisCalib.TransformationY + " lowestprice: " + startPrice + " endprice" + endPrice + " candleWidth: " + candlePropperties.CandleWidth);
            //    MessageBox.Show("start: " + startPrice + " end: " + endPrice);
            bool isGrowing = true;

            if (startPrice - endPrice > 0) isGrowing = false;

            if (isGrowing)
            {
                // candle shadow
                shadow[dayNb] = new Rectangle();
                shadow[dayNb].Width = candlePropperties.CandleWidth / 20;
                shadow[dayNb].Height = (highestPrice - lowestPrice)*axisCalib.GetPixelUnitY();
                shadow[dayNb].Fill = System.Windows.Media.Brushes.DarkGreen;
                canvasChartScreen.Children.Add(shadow[dayNb]);
                Canvas.SetBottom(shadow[dayNb], ((lowestPrice - axisCalib.TransformationY) * axisCalib.GetPixelUnitY() ));
                Canvas.SetLeft(shadow[dayNb], ((dayNb - 1) * axisCalib.GetStepX()) + candlePropperties.CandleWidth / 2);  

                // candle corps
                candle[dayNb] = new Rectangle();
                candle[dayNb].Width = candlePropperties.CandleWidth;
                candle[dayNb].Height = (endPrice - startPrice) * axisCalib.GetPixelUnitY();
                candle[dayNb].Stroke = System.Windows.Media.Brushes.DarkGreen;
                candle[dayNb].StrokeThickness = 2;
              // candle[dayNb].Fill = System.Windows.Media.Brushes.DimGray; //!!!!
               candle[dayNb].Fill = new SolidColorBrush(Color.FromRgb(47, 151, 115)); // !!!!
                canvasChartScreen.Children.Add(candle[dayNb]);
                Canvas.SetBottom(candle[dayNb], (startPrice - axisCalib.TransformationY) * axisCalib.GetPixelUnitY());
                Canvas.SetLeft(candle[dayNb], ((dayNb - 1) * axisCalib.GetStepX())); 


            }
            else
            {
                // candle shadow
                shadow[dayNb] = new Rectangle();
                shadow[dayNb].Width = candlePropperties.CandleWidth / 20;
                shadow[dayNb].Height = (highestPrice - lowestPrice) * axisCalib.GetPixelUnitY();
                shadow[dayNb].Fill = System.Windows.Media.Brushes.DarkRed;
                canvasChartScreen.Children.Add(shadow[dayNb]);
                Canvas.SetBottom(shadow[dayNb], (lowestPrice - axisCalib.TransformationY) * axisCalib.GetPixelUnitY() );
                Canvas.SetLeft(shadow[dayNb], ((dayNb - 1) * axisCalib.GetStepX()) + candlePropperties.CandleWidth / 2);


                // candle corps
                candle[dayNb] = new Rectangle();
                candle[dayNb].Width = candlePropperties.CandleWidth;
                candle[dayNb].Height = (startPrice - endPrice) * axisCalib.GetPixelUnitY(); // cena maleje
                candle[dayNb].Stroke = System.Windows.Media.Brushes.DarkRed;
                candle[dayNb].StrokeThickness = 2;
              //  candle[dayNb].Fill = System.Windows.Media.Brushes.DarkRed;
                candle[dayNb].Fill = new SolidColorBrush(Color.FromRgb(176, 71, 96));
                canvasChartScreen.Children.Add(candle[dayNb]);
                Canvas.SetBottom(candle[dayNb], (endPrice - axisCalib.TransformationY) * axisCalib.GetPixelUnitY()); 
                Canvas.SetLeft(candle[dayNb], ((dayNb - 1) * axisCalib.GetStepX()));

                // candle shadow 
            }
        }

        #endregion







        # region Loading data from file

        private void LoadDataFromFile(ref DataCollector [] dc, int userRange)
        {
            IOAdapter IOAdapt = new IOAdapter();
            IOAdapt.LoadStockData(ref dc, userRange);

        }


        private void SetDataManualy()
        {
            for (int i = 0; i < MAX_USAGE_RANGE; i++)
            {
                dataCollector[i] = new DataCollector();
            }
            /*
            dataCollector[0].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[1].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[2].LoadPieceOfData(250, 230, 180, 265);
            dataCollector[3].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[4].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[5].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[6].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[7].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[8].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[9].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[10].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[11].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[12].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[13].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[14].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[15].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[16].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[17].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[18].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[19].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[19].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[20].LoadPieceOfData(870, 820, 800, 990);
            dataCollector[21].LoadPieceOfData(870, 850, 800, 990);
            dataCollector[22].LoadPieceOfData(850, 800, 760, 990);
            dataCollector[23].LoadPieceOfData(800, 760, 720, 980);
            dataCollector[24].LoadPieceOfData(800, 740, 710, 970);
            dataCollector[25].LoadPieceOfData(700, 680, 630, 660);
            dataCollector[26].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[27].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[28].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[29].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[30].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[31].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[32].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[33].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[34].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[35].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[36].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[37].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[38].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[39].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[40].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[41].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[42].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[43].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[44].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[45].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[46].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[47].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[48].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[49].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[50].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[51].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[52].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[53].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[54].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[55].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[56].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[57].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[58].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[59].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[60].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[61].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[62].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[63].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[64].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[65].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[66].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[67].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[68].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[69].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[70].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[71].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[72].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[73].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[74].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[75].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[76].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[77].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[78].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[79].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[80].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[81].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[82].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[83].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[84].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[85].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[86].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[87].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[88].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[89].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[90].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[91].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[92].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[93].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[94].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[95].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[96].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[97].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[98].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[99].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[100].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[101].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[102].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[103].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[104].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[105].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[106].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[107].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[108].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[109].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[110].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[111].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[112].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[113].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[114].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[115].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[116].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[117].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[118].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[119].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[119].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[120].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[121].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[122].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[123].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[124].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[125].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[126].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[127].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[128].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[129].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[130].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[131].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[132].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[133].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[134].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[135].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[136].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[137].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[138].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[139].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[140].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[141].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[142].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[143].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[144].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[145].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[146].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[147].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[148].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[149].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[150].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[151].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[152].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[153].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[154].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[155].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[156].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[157].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[158].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[159].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[160].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[161].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[162].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[163].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[164].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[165].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[166].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[167].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[168].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[169].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[170].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[171].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[172].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[173].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[174].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[175].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[176].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[177].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[178].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[179].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[180].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[181].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[182].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[183].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[184].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[185].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[186].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[187].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[188].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[189].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[190].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[191].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[192].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[193].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[194].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[195].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[196].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[197].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[198].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[199].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[200].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[201].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[202].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[203].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[204].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[205].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[206].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[207].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[208].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[209].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[210].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[211].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[212].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[213].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[214].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[215].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[216].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[217].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[218].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[219].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[220].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[221].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[222].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[223].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[224].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[225].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[226].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[227].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[228].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[229].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[230].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[231].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[232].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[233].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[234].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[235].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[236].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[237].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[238].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[239].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[240].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[241].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[242].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[243].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[244].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[245].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[246].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[247].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[248].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[249].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[250].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[251].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[252].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[253].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[254].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[255].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[256].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[257].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[258].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[259].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[260].LoadPieceOfData(900, 950, 880, 990);
            dataCollector[261].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[262].LoadPieceOfData(870, 990, 800, 990);
            dataCollector[263].LoadPieceOfData(850, 960, 760, 990);
            dataCollector[264].LoadPieceOfData(800, 950, 720, 980);
            dataCollector[265].LoadPieceOfData(800, 950, 710, 970);
            dataCollector[266].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[267].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[268].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[269].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[270].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[271].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[272].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[273].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[274].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[275].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[276].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[277].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[278].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[279].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[280].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[281].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[282].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[283].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[284].LoadPieceOfData(200, 250, 180, 300);
            dataCollector[285].LoadPieceOfData(200, 250, 190, 260);
            dataCollector[286].LoadPieceOfData(250, 260, 180, 265);
            dataCollector[287].LoadPieceOfData(270, 290, 180, 340);
            dataCollector[288].LoadPieceOfData(300, 350, 280, 400);
            dataCollector[289].LoadPieceOfData(400, 450, 380, 460);
            dataCollector[290].LoadPieceOfData(400, 450, 380, 470);
            dataCollector[291].LoadPieceOfData(450, 460, 385, 580);
            dataCollector[292].LoadPieceOfData(470, 490, 420, 500);
            dataCollector[293].LoadPieceOfData(500, 550, 460, 600);
            dataCollector[294].LoadPieceOfData(600, 650, 540, 700);
            dataCollector[295].LoadPieceOfData(600, 650, 570, 720);
            dataCollector[296].LoadPieceOfData(650, 660, 480, 710);
            dataCollector[297].LoadPieceOfData(670, 690, 480, 700);
            dataCollector[298].LoadPieceOfData(700, 950, 630, 960);
            dataCollector[299].LoadPieceOfData(800, 950, 710, 970);
             */
        }

        #endregion


        #region Loading Data From DataBase

        private void LoadDataFromDataBase(ref DataCollector[] dc, int Range, string stockName) 
         {
            string _marketType="";
            if (GpwButtonOn == true) { _marketType = POLISH_STOCKS; }
            if (TfiButtonOn == true) { _marketType = POLISH_FUNDS; }
            IOFileAdapter IoFileadapt = new IOFileAdapter(_marketType);
            IoFileadapt.GetMarketData(Range, ref dc, stockName);
         }

        #endregion


        #region Communication with option windows

        public void MainWindowStockToFind(string stockName)
        {
            MainLoop(ref GlobalOldUserRange, ref GlobalUserRange, stockName);
        }

        #endregion


        #region Other unclasified methods

        private void DataCollectorObjectInit(int userRange)
        {
            for (int i = 0; i < userRange; i++)
            {
                dataCollector[i] = new DataCollector();
            }
        }

        // sending back market that was choosen by user by proper button
        private string GiveMeChoosedMarket()
        {
            string _marketChoosen = "";
            if (GpwButtonOn == true) { _marketChoosen = POLISH_STOCKS; }
            if (TfiButtonOn == true) { _marketChoosen = POLISH_FUNDS; }
            return (_marketChoosen);
        }

        #endregion



    }
}
