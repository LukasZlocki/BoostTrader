using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoostTrader2._0.Grid
{
    class PriceEvaluation
    {
        private double dMaxPriceYaxis; // max value evaluated from data 
        private double dMinPriceYaxis; // max value evaluated from data 


        private double dPriceRange; // price from data max price - min price gives range

        private double dPricePerPixel; // price per pixel



        // Constructor
        public PriceEvaluation (double maxPrice, double minPrice, double screenHeight)
        {
            this.dMinPriceYaxis = minPrice;
            this.dMaxPriceYaxis = maxPrice;

            SetPriceRange(maxPrice, minPrice);
            SetPricePerPixel(dPriceRange, screenHeight);
        }


        /// <summary>
        /// 
        /// </summary>
        public double GetPriceOnThisPixel(double _Ypossition)
        {
            double _price;

            //starting calculation from min pricie ! 
            _price = dMinPriceYaxis + _Ypossition * dPricePerPixel; 
            return (_price);
        }

        private void SetPricePerPixel(double _priceRange, double _screenHeight)
        {
            this.dPricePerPixel = _priceRange / _screenHeight;
        }

        private void SetPriceRange (double _maxPrice, double _minPrice)
        {
            this.dPriceRange = _maxPrice - _minPrice;
        } 

    }
}
