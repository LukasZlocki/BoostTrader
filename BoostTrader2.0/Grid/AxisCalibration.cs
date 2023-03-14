using BoostTrader2._0.IOAdapters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


using BoostTrader2._0.ChartsLib;

namespace BoostTrader2._0.ChartsLib
{
    /// <summary>
    /// Calculating steps :
    /// Step I : Gathering canvas screen size / Method : SetScreenSize(double xSize, double ySize) - 
    /// Step II : gatherig pack of data / Method : SetData(DataCollector[] dc, int rangeOfData)
    ///        - step II.1 : Gathering charts data (ex close value, start value, etc) data from DataCollector
    ///        - step II.2 : Auto scalling Y Axis by calculating min/max value, step, range
    ///        - step II.3 : Auto scalling X Axis by calculating step
    ///        - step II.4 : Calcualte transformation factor on Y Axis to transform candle possition according to autoscalled Y axis. 
    /// </summary>

    public class AxisCalibration
    {
        private static string X_AXIS = "X_AXIS";
        private static string Y_AXIS = "Y_AXIS";

        private static int CANDLE_SEPARATION = 1; // separation 1 px    <- ta zmienna wrzucilem do klasy dziedziczacej - jesli wszystko OK skasowac ja tutaj

        private double ScreenSizeX; // X size in px
        private double ScreenSizeY; // Y size in px



        protected double MaxValueYaxis; // max value evaluated from data 
        protected double MinValueYaxis; // min value evaluated from data

        protected double MaxValueXaxis; // max value evaluated from data 
        protected double MinValueXaxis; // min value evaluated from data

        /* This parameters can be use in future for rescaling axis  
        protected int ModifiedMaxValueYaxis; //max value show on label Y which can be divided by 5
        protected int ModifiedMinValueYaxis; // min value show on label Y which can be divided by 5
        */
     

        protected double RangeY; // calculated max - min as a RangeY

        protected double PixelUnitY;      // <-- value calculated by dividing ScreenSizeY by RangeY ,  
        
        protected double StepX;
        protected double ProbeQuantityX;

        public double TransformationY {get; private set; }

        //  public double CandleWidth; <- ta zmienna wrzucilem do klasy dziedziczacej - jesli wszystko OK skasowac ja tutaj


      


        // *********** METHODS ***********************************

        private void CalcualteTransformationY()
        {
            this.TransformationY = this.MinValueYaxis;           
        }


        public void SetData(DataCollector[] dc, int rangeOfData)
        {
            this.ProbeQuantityX = rangeOfData;
            MinMaxValueCalculationForYaxis(dc, rangeOfData);

            /* This operation can be use in future for rescaling axis  
            ModifiedMinValueYaxis = DecreaseValueToDivideBy5(Convert.ToInt32(MinValueYaxis));
            ModifiedMaxValueYaxis = IncreaseValueToDivideBy5(Convert.ToInt32(MaxValueYaxis));
            */
             
            StepAndRangeCalculation(this.ScreenSizeY,this.MaxValueYaxis, this.MinValueYaxis, Y_AXIS);
            StepAndRangeCalculation(this.ScreenSizeX, this.MaxValueXaxis, this.MinValueXaxis, X_AXIS);
          
            CalcualteTransformationY();
        }


        // setting canvas screen resolution data
        public void SetScreenSize(double xSize, double ySize)
        {
            this.ScreenSizeX = xSize;
            this.ScreenSizeY = ySize;
            
        }


        // calculating min, max value for Y Axis 
        private void MinMaxValueCalculationForYaxis(DataCollector[] dcol, int rangeOfData)
        {

            double tmin, tmax;
            tmin = dcol[0].GetEndPrice();
            tmax = dcol[0].GetEndPrice();

        //    for (int i = 0; i < rangeOfData; i++)
            for (int i = 0; i < rangeOfData; i++)
            {
                if (dcol[i].GetEndPrice() > tmax) tmax = dcol[i].GetEndPrice();
                if (dcol[i].GetStartPrice() > tmax) tmax = dcol[i].GetStartPrice();
                if (dcol[i].GetMaxPrice() > tmax) tmax = dcol[i].GetMaxPrice();
                if (dcol[i].GetMinPrice() > tmax) tmax = dcol[i].GetMinPrice();

                if (dcol[i].GetEndPrice() < tmin) tmin = dcol[i].GetEndPrice();
                if (dcol[i].GetStartPrice() < tmin) tmin = dcol[i].GetStartPrice();
                if (dcol[i].GetMaxPrice() < tmin) tmin = dcol[i].GetMaxPrice();
                if (dcol[i].GetMinPrice() < tmin) tmin = dcol[i].GetMinPrice();
            }
            this.MinValueYaxis = tmin;
            this.MaxValueYaxis = tmax;

        }


       // Step and range calculation
        private void StepAndRangeCalculation(double screenSize, double maxValueAxis,double minValueAxis, string XorY)
        {
            if (XorY == "X_AXIS")
            {
                this.StepX = Math.Round((screenSize / this.ProbeQuantityX), 2);               
            }

            if (XorY == "Y_AXIS")
            {
                this.RangeY = maxValueAxis - minValueAxis;
                this.PixelUnitY = Math.Round ((screenSize / this.RangeY), 2);              
            }
        }

        /// <summary>
        /// Calculate pixel possition in Y Axis by price given by user
        /// </summary>
        /// <param name="price"></param>
        /// <param name="pixelUnit"></param>
        /// <param name="minValueYaxis"></param>
        /// <returns>  </returns>
        private double YaxisPixelPosByPrice(double price, double pixelUnit, double minValueYaxis)
        {
            double _pixelToReturn = 0;
            double _calcValue = 0;

            _calcValue = price - minValueYaxis; // calculation with assumtion that minValueYaxis is zero point for pixel pos calculation
            _pixelToReturn = _calcValue * pixelUnit;   // _calcValue multiply by pixel per price unit

            return (_pixelToReturn);
        }


        /* This operation can be use in future for rescaling axis  
        
        #region Reducing/Increasing values to get one dividing by 5
        private int DecreaseValueToDivideBy5(int nbToConvert)
        {
            int _value;
            int _modulo = 1;

            nbToConvert++;

            do
            {
                nbToConvert--;
                _modulo = nbToConvert % 5;

            } while (!(_modulo == 0));

            _value = nbToConvert;
            return (_value);
        }


        private int IncreaseValueToDivideBy5(int nbToConvert)
        {
            int _value;
            int _modulo = 1;

            nbToConvert--;

            do
            {
                nbToConvert++;
                _modulo = nbToConvert % 5;

            } while (!(_modulo == 0));

            _value = nbToConvert;
            return (_value);
        }
        #endregion
        */
   
        #region Getters

        public double GetMinValueY()
        {
            return this.MinValueYaxis;
        }

        public double GetMaxValueY()
        {
            return this.MaxValueYaxis;
        }

        public double GetRangeY()
        {
            return this.RangeY;
        }

        public double GetStepY()
        {
            return this.PixelUnitY;
        }

        
        public double GetStepX()
        {
            return this.StepX;
        }
        
        public double GetProbeQuantityX()
        {
            return this.ProbeQuantityX;
        }
                  
        public double GetScreenSizeX()
        {
            return this.ScreenSizeX;
        }
         
        public double GetScreenSizeY()
        {
            return this.ScreenSizeY;
        }

        public double GetPixelUnitY()
        {
            return this.PixelUnitY;
        }

        public double GetYaxisPixelPosByPrice(double price)
        {
            double _YaxisPixelToReturn = 0;
            _YaxisPixelToReturn = YaxisPixelPosByPrice(price, PixelUnitY, MinValueYaxis);
            return (_YaxisPixelToReturn);
        }
        #endregion


   

    }

  }

