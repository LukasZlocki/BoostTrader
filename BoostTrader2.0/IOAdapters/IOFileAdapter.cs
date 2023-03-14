using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;

namespace BoostTrader2._0.IOAdapters
{
    class IOFileAdapter
    {
        // Static paths with data
        private static string POLISH_STOCKS_PATH = @"Datas\Stocks\";
        private static string POLISH_FUNDS_PATH = @"Datas\Funds\";
        private static string FILE_EXTENSION = ".txt";
      
        // Paths with data
        private string PathTFI = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), POLISH_FUNDS_PATH);
        private string PathGPW = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), POLISH_STOCKS_PATH);
        private string GlobalPath = @"";

        // Lists used at HetGPWData method
        List<DataCollector> dcList = new List<DataCollector>();
        List<DataCollector> dcListFinal = new List<DataCollector>();

        // Constructor 
        public IOFileAdapter(string market)
        {
            if (market == "GPW") { GlobalPath = PathGPW; }
            if (market == "TFI") { GlobalPath = PathTFI; }
        }

        public void GetMarketData(int nProbe, ref DataCollector[] dc_, string stockName)
        {      
            ExtractAllDataFromFile(stockName, ref dcList);
            SetExtractedDataDescending (ref dcList);
            CutDataAccordingToUserDataRange(ref dcList, nProbe, ref dcListFinal);
            OrderFinalListToAscending(ref dcListFinal);
            TransferFinalListToReferencObject(ref dc_, ref dcListFinal);
        }

        // check dir and put names of files into list , the names in fact are names of GPW companies
        public void GetDirectoryFilesNameList(ref List<FileNameCollector> namesCollector)
        {
            namesCollector = new List<FileNameCollector>();
            string[] fileArray = Directory.GetFiles("C:\\Documents", "*.txt"); // <<--- TODO : a co to jest za katalog ?
            
            for (int i = 0; i < fileArray.Length; i++ )
            {
                namesCollector.Add(new FileNameCollector()
                {
                    DirFileName = fileArray[i]
                });
            }
        }


        #region Methods for GetMarketData

        private void ExtractAllDataFromFile(string sValue, ref List<DataCollector> dcFileDataList)
        {
            try
            {
                string _string = "";

                bool _isThisfirstLineToDelete = true;
                bool _ReadyToAddDatatoList = false;

                int _column = -1;
                char ch;

                string _Name = "";
                string _Date = "";
                double _StartP = 0;
                double _EndP = 0;
                double _MaxP = 0;
                double _MinP = 0;
                int _Vol = 0;

                StreamReader sr = new StreamReader(""+GlobalPath+sValue+FILE_EXTENSION);
                do
                {

                    ch = (char)sr.Read();

                    if ( (ch == ',' && _isThisfirstLineToDelete == false) || (_column == 6 && ch == '\r') )
                    {
                        if (_column == 0) { _Name = _string; _string = ""; }
                        if (_column == 1) { _Date = _string; _string = ""; }
                        if (_column == 2) { _StartP = double.Parse(_string, System.Globalization.CultureInfo.InvariantCulture); _string = ""; }
                        if (_column == 3) { _MaxP = double.Parse(_string, System.Globalization.CultureInfo.InvariantCulture); _string = ""; }
                        if (_column == 4) { _MinP = double.Parse(_string, System.Globalization.CultureInfo.InvariantCulture); ; _string = ""; }
                        if (_column == 5) { _EndP = double.Parse(_string, System.Globalization.CultureInfo.InvariantCulture); _string = ""; }
                        if ( (_column == 6) && (ch == '\r')) { _Vol = int.Parse(_string); _string = ""; _ReadyToAddDatatoList = true; _column = -1; }

                        _column++;
                    }

                    // adding data to list and reseting all information
                    if (_ReadyToAddDatatoList == true )
                    {
                        dcFileDataList.Add(new DataCollector()
                            {
                                Name = _Name,
                                Date = _Date,
                                StartPrice = _StartP,
                                EndPrice = _EndP,
                                MaxPrice = _MaxP,
                                MinPrice = _MinP,
                                Volume = _Vol
                            });
                        
                        _ReadyToAddDatatoList = false; // waiting for new readiness
                        _Name = "";
                        _Date = "";
                        _StartP = 0;
                        _EndP = 0;
                        _MaxP = 0;
                        _MinP = 0;
                        _Vol = 0;
                    }

                    // adding next char to string
                    if (!((ch == '\r') || (ch == '\n') || (ch == ',')))
                    {
                        _string = _string + Convert.ToString(ch);
                    }

                    // first line has just apeared and is scraped <-- works only if first line is red and need to be trashed
                    if (ch == '\n' && _isThisfirstLineToDelete == true) { _isThisfirstLineToDelete = false; _string = ""; _column = 0; }

                } while (!sr.EndOfStream);
            }
            catch
            {
                // TODO : Write thread informatin here
            }
        }
   
        private void SetExtractedDataDescending (ref List<DataCollector> dcDescendList)
        {
           //  dcDescendList.OrderByDescending(x => x.Date).Cast<DataCollector>().ToList();

             List<DataCollector> li;
             li = dcDescendList.OrderByDescending(x => x.Date).Cast<DataCollector>().ToList();
             dcDescendList = li;
        }

        private void CutDataAccordingToUserDataRange(ref List<DataCollector> dcToCutList, int nProbe, ref List<DataCollector> finalList)
        {
            int i = 0;
            foreach (DataCollector dc in dcToCutList)
            {

                finalList.Add(new DataCollector() 
                { 
                  Name = dc.Name, 
                  Date = dc.Date,
                  StartPrice = dc.StartPrice,
                  EndPrice = dc.EndPrice,
                  MaxPrice = dc.MaxPrice,
                  MinPrice = dc.MinPrice,
                  Volume = dc.Volume
                });

                if (i == nProbe) { break; }
                i++;
            }

        }

        private void OrderFinalListToAscending(ref List<DataCollector> dcListToOrder)
        {
            List<DataCollector> li;
            li = dcListToOrder.OrderBy(x => x.Date).Cast<DataCollector>().ToList();
            dcListToOrder = li;
        }

        private void TransferFinalListToReferencObject(ref DataCollector [] dataColector, ref List<DataCollector> dcListToTransfer)
        {
            int i = 0;
             foreach (DataCollector _dc in dcListToTransfer)
            {
                dataColector[i] = new DataCollector();
                dataColector[i].Name = _dc.Name;
                dataColector[i].Date = _dc.Date;
                dataColector[i].StartPrice = _dc.StartPrice;
                dataColector[i].EndPrice = _dc.EndPrice;
                dataColector[i].MaxPrice = _dc.MaxPrice;
                dataColector[i].MinPrice = _dc.MinPrice;
                dataColector[i].Volume = _dc.Volume;

                i++;
            }

        }

        #endregion

    }
}
