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


using System.IO;
using System.Reflection;

namespace BoostTrader2._0
{
    /// <summary>
    /// 
    /// </summary>
    public partial class StockChoice : Window
    {
        // Static paths with data
        private static string POLISH_STOCKS_PATH = @"Datas\Stocks\";
        private static string POLISH_FUNDS_PATH = @"Datas\Funds\";
        private string FILE_EXTENSION = ".txt";

        // Paths with data
        private string PathTFI = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), POLISH_FUNDS_PATH);
        private string PathGPW = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), POLISH_STOCKS_PATH);
        private string GlobalPath = @"";

        //  MainWindow mainWindowCommunicator = new MainWindow();

        MainWindow main;

        public StockChoice(MainWindow main, string market)
        {
            InitializeComponent();

            this.main = main;

            if (market == "GPW") { GlobalPath = PathGPW; }
            if (market == "TFI") { GlobalPath = PathTFI; }
    
            CollectFileNames("", GlobalPath);
        }

        #region ListBox instant look up
       
        private void InstantLookUp(object sender, KeyEventArgs e)
        {
            string _stringToFind;

            _stringToFind = txtStringToFind.Text;
            CollectFileNames(_stringToFind, GlobalPath);
        }

        // clearing the list
        private void ClearListBoxList(ref ListBox lBox)
        {
            lBox.Items.Clear();
        }

        #endregion


        #region Collecting stocks list 
       
        private void CollectFileNames(string stringTofind, string globalPath)     
        {
            // clearing listboxlist
            ClearListBoxList(ref LboxList);

            string _fileNameOnly="";
            //string GlobalPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Datas\");

            string[] filenames = Directory.GetFiles(""+globalPath, stringTofind+"*");
            foreach (string filename1 in filenames)
            {
                _fileNameOnly = ConvertToFileNameOnly(filename1);
                LboxList.Items.Add(_fileNameOnly);
            }

        }

        #endregion


        # region ListBox - choosing stocks
        // pass clicked data at Listbox to generate new chart at MainWindow
        private void LboxList_Click(object sender, MouseButtonEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            string filename = listbox.SelectedItem.ToString();
            
            // sending stock selected by user to MainWindow
            SendStockNameToMainWindow(filename);

        }

        // converting string with path and file name with extension to file name ONLY
        private string ConvertToFileNameOnly(string allPathWithFileName)
        {
            string _nameToReturn = "";
            string _fileNameWithExtension = "";

            // Step I : removing path and living only file name with extension
            _fileNameWithExtension = Path.GetFileName(allPathWithFileName); 

            // Step II : Remove extension from file name
            _nameToReturn = _fileNameWithExtension.Replace(FILE_EXTENSION,"");

            return(_nameToReturn);
        }
        #endregion


        private void  SendStockNameToMainWindow(string _stockName)
        {
            this.Close();
            this.main.MainWindowStockToFind(_stockName);                      
        }


        #region Buttons

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion


    }
}
