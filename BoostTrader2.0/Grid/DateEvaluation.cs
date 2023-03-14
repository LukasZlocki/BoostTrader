using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoostTrader2._0.IOAdapters;

namespace BoostTrader2._0.Grid
{

    /// <summary>
    /// Function of this class is to establish date of price pointed by user 
    /// 
    /// !! Tutaj opisac krok po kroku jak wyznaczana jest data - potrzeba opisania logiki !!
    /// 
    /// Step I :
    /// Creating object with probe quantity / screen width / DataCollector
    /// 
    /// Step II :
    /// Establishe date per each pixel (stablishDatePerPixel)
    /// 
    /// How to get date by pixel possition ?
    /// a. get pixel possition and divide it by date per pixel
    /// b. results of dividing is probe number 
    /// c. get probe number value from DataCollector
    /// 
    ///
    /// </summary>

    class DateEvaluation
    {
        private double dDatePerPixel;
        private int nProbe;
        DataCollector [] dataCollector;


        // constructor
        public DateEvaluation(int probe, double screenWidth, DataCollector [] dc)
        {
            this.nProbe = probe;
            dataCollector = dc;

            EstablishDatePerPixel(nProbe, screenWidth);
        }



        // Calculation to establish date per each pixel on screen , to do so we need probe and screen width
        private void EstablishDatePerPixel(int probe, double screenWidth)
        {
            this.dDatePerPixel = screenWidth / probe;
        }


        // Getting date from DataCollector by giving user X Possition
        public string GetDateByPossition (double xPos)
        {
            string _dateToReturn="";
            int _establishedProbeNb;

            _establishedProbeNb = GetProbeNb(xPos, dDatePerPixel);

           _dateToReturn = dataCollector[_establishedProbeNb].GetData();

            return(_dateToReturn);
        }


        // getting probe nb by dividing user xPos by datePerPixel
        private int GetProbeNb(double xPos, double datePerPixel)
        {
            int _probeNb;
            _probeNb = (int)(xPos / datePerPixel);
            return (_probeNb);
        }

    }
}
