using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BoostTrader2._0.IOAdapters
{
    class IOAdapter
    {


        #region Loading stock data
        public void LoadStockData(ref DataCollector [] dc, int userRange)
        {           
            try
            {
                using (StreamReader sr = new StreamReader("StockData.txt"))
                {
                    string _line;
                    int _pointer = 0;
                    int _counter = 0;
                    bool _stopCounting = false;
                    bool _isInitialised = false;

                    while ( (_line = sr.ReadLine()) != null || (_counter == userRange) )
                    {

                        if (!(_isInitialised)) { dc[_counter] = new DataCollector(); _isInitialised = true; }

                        if (_pointer == 0) { dc[_counter].Date = _line; }
                        if (_pointer == 1) { dc[_counter].StartPrice = Convert.ToDouble(_line); }
                        if (_pointer == 2) { dc[_counter].EndPrice = Convert.ToDouble(_line); }
                        if (_pointer == 3) { dc[_counter].MinPrice = Convert.ToDouble(_line); }
                        if (_pointer == 4) 
                        {
                            dc[_counter].MaxPrice = Convert.ToDouble(_line);
                            _pointer = 0;
                            _counter++;
                            _stopCounting = true;
                            _isInitialised = false;
                        }

                        if (_stopCounting) { _pointer = 0; _stopCounting = false; }
                        else { _pointer++; }
                    }
                }
            }
            catch (Exception e)
            {
                // TODO : write exception to create file 
            }
        }

        #endregion




    }
}
