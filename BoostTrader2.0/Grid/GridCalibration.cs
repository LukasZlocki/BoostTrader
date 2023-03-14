using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// Idea działania programu :
/// Y Axis :
/// Step I & II - min & max value uppr/lower to number that can be divided by 5
/// Step III : Establish range between new min / max value
/// Step IV : Establish QuantityOfSteps and UnitsPerStep by dividing range by steps, where steps are 3 -> 8 , 
/// 
/// X Axis 
/// -> ustawić sztucznie w zależności od przedziału 1m, 3m, 6m, 1y, 2y, 3y ,5y , 10y .... ustalic przedzialy (!)
/// 
/// <summary>
namespace BoostTrader2._0.ChartsLib 
{
    public class GridCalibration
    {
        private static int MIN_STEP_ON_Y_AXIS = 3;
        private static int MAX_STEP_ON_Y_AXIS = 10;

        private int [] DivideMatrix = new int[] {1, 2, 5, 10, 100, 1000};

        private int nModifiedMinValueYAxis = 0; // min value show on label Y which can be divided by 5
        private int nModifiedMaxValueYAxis = 0; //max value show on label Y which can be divided by 5
        private int nModifiedRangeYAxis; // minimalized max - maxymalized min and results reduced to neares number that can be divided by 5 without rest. 
        private int nQuantityOfStepsYAxis; // How many steps we have on Y - Axis
        private int nUnitsPerStepYAxis; // units per each step

      //  private double dValuePerEachUnitInStep = 0; // value of resolution of each step 

     


        // Constructor
        public GridCalibration (double minValue, double maxValue)
        {
            bool _gridCalibrated = false;

           
            int _minY = Convert.ToInt32(minValue);
            int _maxY = Convert.ToInt32(maxValue);
            int _range = +_maxY - _minY;

            StartCalibrationOfGrid(_minY, _maxY);
        }

        //  TODO : poprawic kod i ogarnac liczb y < 1
        private void StartCalibrationOfGrid (int nMinY, int nMaxY )
        {
            bool _gridEstablished = false;
            
            // idea of the loop is to look up through divide matrix and choose best (max 8 steps) to draw chatch with prices
            int _divideBy;
            int _increment = 0;
            do
            {
                _divideBy = DivideMatrix[_increment]; 

            // Step I & II - min & max value uppr/lower to number that can be divided by 5
            this.nModifiedMinValueYAxis = IncreaseValueToDivideByX(nMinY, _divideBy); ;
            this.nModifiedMaxValueYAxis = DecreaseValueToDivideByX(nMaxY, _divideBy);

            // Step III : Establish range between new min / max value 
            this.nModifiedRangeYAxis = nModifiedMaxValueYAxis - nModifiedMinValueYAxis;

            // Step IV : Establish QuantityOfSteps and UnitsPerStep by dividing range by steps, where steps are 3 -> 8 ,         
            _gridEstablished = EstablishingStepsQuantityAndUnitsPerStep(nModifiedRangeYAxis, ref nUnitsPerStepYAxis, ref nQuantityOfStepsYAxis);

            _increment++;
            } while (!(_gridEstablished));
                                        
        }


        # region Establishind Steps and UnitsPerStep
        private bool EstablishingStepsQuantityAndUnitsPerStep(int nRange, ref int nUnitPerSteps, ref int nQuanityOfSteps)
        {
            int _temporaryValue;

            for (int i = MAX_STEP_ON_Y_AXIS; i > MIN_STEP_ON_Y_AXIS; i--)
            {
                _temporaryValue = nRange % i; // -> check if integer

                if (_temporaryValue == 0 && i >= MIN_STEP_ON_Y_AXIS) // is integer ! 
                {
                    nQuanityOfSteps = i;  // quantity of steps established
                    nUnitPerSteps = nRange / i; // quantity of units in each step established    
                    return (true);
                }
            }
            return (false);
        }
        # endregion
      

        # region Reducing/Increasing values to get one dividing by X

        private int DecreaseValueToDivideByX (int nbToConvert, int divideBy)
        {
            int _value;
            int _modulo = 1;

            nbToConvert++;

            do
            {
                nbToConvert--;
                _modulo = nbToConvert % divideBy;

            } while (!(_modulo == 0));

            _value = nbToConvert;
            return (_value);
        }

        private int IncreaseValueToDivideByX (int nbToConvert, int divideBy)
        {
            int _value;
            int _modulo = 1;

            nbToConvert--;


            do
            {
                nbToConvert++;
                _modulo = nbToConvert % divideBy;

            } while (!(_modulo == 0));

            _value = nbToConvert;
            return (_value);
        }

        #endregion
   

        #region - Get Labels and its values 

        public double GetLabelValue(int stepToShowValueOnLabel)
        {
            double _dShowThisValue = 0;

            for (int i = 0; i < stepToShowValueOnLabel; i++ )
            {
               // _dShowThisValue = nUnitsPerStepYAxis + i * nUnitsPerStepYAxis;
                _dShowThisValue = nModifiedMinValueYAxis + i * nUnitsPerStepYAxis;
            }

               return (_dShowThisValue);
        }

        #endregion


        #region Getters

        public int GetMin()
        {
            return (this.nModifiedMinValueYAxis);
        }

        public int GetMax()
        {
            return (this.nModifiedMaxValueYAxis);
        }

        public int GetQuantityOfStepsYAxis()
        {
            return (this.nQuantityOfStepsYAxis);
        }

        public int GetUnitsPerStepYAxis()
        {
            return (this.nUnitsPerStepYAxis);
        }

        #endregion




    }
}
