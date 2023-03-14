using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoostTrader2._0.ChartsLib
{
    public class CandleChart : AxisCalibration
    {
        /// <summary>
        /// Methods:
        /// - Candle Width Calculation
        /// </summary>



        public static int CANDLE_SEPARATION = 1; // separation 1 px

        public double CandleWidth { get; private set; }
       

        // Candle width calculation
        public void CandleWidthCalculation(double probeQuantityX, double screensizeX)
        {           
            double spaceForCandle;
            double candleWidth;
            spaceForCandle = screensizeX / probeQuantityX;
            candleWidth = spaceForCandle - (2 * CANDLE_SEPARATION);
            this.CandleWidth = candleWidth;
        }


        #region Getters

        /* <- zerknij w implementacje zmienne j jest tam private set co uniemozliwe ustawienie tej mziennej na zewnątrz klasy
        public double GetCandleWidth() {
            return this.CandleWidth;
        }
        */
         
        /* <- zerknij w implementacje zmienne j jest tam private set co uniemozliwe ustawienie tej mziennej na zewnątrz klasy
        public int GetCandleSeparation {  
            return this.CANDLE_SEPARATION
        }

         * */

        #endregion;



    }
}
