using System;
using System.Windows.Forms;


namespace WebProxy
{
    /// <summary>
    /// Contains the methods used for interacting with the management console.
    /// Also contains a lot of automatically generated code
    /// </summary>
    public partial class ManagementConsole : Form
    {
        /// <summary>
        /// Delegate for updating a string between threads
        /// </summary>
        /// <param name="text">Text to be changed</param>
        delegate void StringArgReturningVoidDelegate(string text);


        /// <summary>
        /// Delegate for updating an int between threads
        /// </summary>
        /// <param name="byteNum">Int to be changed</param>
        delegate void IntArgReturningVoidDelegate(int byteNum);

        /// <summary>
        /// Default strings for the various text boxes
        /// </summary>
        private readonly string CacheSizeTextBoxDefaultText = "Cache Size: ";
        private readonly string RequestCountTextBoxDefaultText = "Total HTTP GET requests: ";
        private readonly string CacheTimingTextBoxDefaultText = "Average Time per Request: ";
        private readonly string BytesFromServerTextBoxDefaultText = "Total Bytes Sent from Server: ";
        private readonly string BytesFromCacheTextBoxDefaultText = "Total Bytes Sent from Cache: ";
        private readonly string AverageBytesTextBoxDefaultText = "Average Bytes per Request: ";


        /// <summary>
        /// Constructor
        /// </summary>
        public ManagementConsole()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Update the value of the cache size text box
        /// </summary>
        /// <param name="text">New text to be inserted</param>
        public void UpdateCacheSizeTextBox(string text)
        {
            if (this.cacheSizeTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate del = new StringArgReturningVoidDelegate(UpdateCacheSizeTextBox);
                this.Invoke(del, new object[] { text });
            }
            else
            {
                cacheSizeTextBox.Text = CacheSizeTextBoxDefaultText + text;
            }
        }


        /// <summary>
        /// Update the value of the request count text box
        /// </summary>
        /// <param name="text">New text to be inserted</param>
        public void UpdateRequestCountTextBox(string text)
        {
            if (this.requestCountTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate del = new StringArgReturningVoidDelegate(UpdateRequestCountTextBox);
                this.Invoke(del, new object[] { text });
            }
            else
            {
                requestCountTextBox.Text = RequestCountTextBoxDefaultText + text;
            }
        }


        /// <summary>
        /// Update the value of the cache timing text box
        /// </summary>
        /// <param name="text">New text to be inserted</param>
        public void UpdateCacheTimingTextBox(string text)
        {
            if (this.cacheTimingTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate del = new StringArgReturningVoidDelegate(UpdateCacheTimingTextBox);
                this.Invoke(del, new object[] { text });
            }
            else
            {
                cacheTimingTextBox.Text = CacheTimingTextBoxDefaultText + text;
            }
        }


        /// <summary>
        /// Update the value of the total bytes from server text box
        /// </summary>
        /// <param name="text">New text to be inserted</param>
        public void UpdateBytesFromServerTextBox(int byteNum)
        {
            if (this.bytesFromServerTextBox.InvokeRequired)
            {
                IntArgReturningVoidDelegate del = new IntArgReturningVoidDelegate(UpdateBytesFromServerTextBox);
                this.Invoke(del, new object[] { byteNum });
            }
            else
            {
                bytesFromServerTextBox.Text = BytesFromServerTextBoxDefaultText + ConvertDataSuffix(byteNum);
            }
        }


        /// <summary>
        /// Update the value of the average bytes from server text box
        /// </summary>
        /// <param name="byteNum">New int to be inserted</param>
        public void UpdateAverageBytesTextBox(int byteNum)
        {
            if (this.averageBytesTextBox.InvokeRequired)
            {
                IntArgReturningVoidDelegate del = new IntArgReturningVoidDelegate(UpdateAverageBytesTextBox);
                this.Invoke(del, new object[] { byteNum });
            }
            else
            {
                averageBytesTextBox.Text = AverageBytesTextBoxDefaultText + ConvertDataSuffix(byteNum);
            }
        }


        /// <summary>
        /// Update the value of the total bytes from cache text box
        /// </summary>
        /// <param name="byteNum">New int to be inserted</param>
        public void UpdateBytesFromCacheTextBox(int byteNum)
        {
            if (this.bytesFromCacheTextBox.InvokeRequired)
            {
                IntArgReturningVoidDelegate del = new IntArgReturningVoidDelegate(UpdateBytesFromCacheTextBox);
                this.Invoke(del, new object[] { byteNum });
            }
            else
            {
                bytesFromCacheTextBox.Text = BytesFromCacheTextBoxDefaultText + ConvertDataSuffix(byteNum);
            }
        }


        /// <summary>
        /// Update the value of the total bytes from server text box
        /// </summary>
        /// <param name="text">New text to be inserted</param>
        public void UpdateRequestsTextBox(string text)
        {
            if (this.requestsMadeTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate del = new StringArgReturningVoidDelegate(UpdateRequestsTextBox);
                this.Invoke(del, new object[] { text });
            }
            else
            {
                requestsMadeTextBox.Text += text;
            }
        }


        /// <summary>
        /// Suffixes for the first data sizes
        /// </summary>
        private static readonly string[] DataSuffixes = { "B", "KB", "MB", "GB", "TB" };


        /// <summary>
        /// Convert a byte value to an appropriate data unit
        /// </summary>
        /// <param name="byteNum">Number of bytes</param>
        /// <returns>String representation of the data using an appropriate unit</returns>
        private string ConvertDataSuffix(int byteNum)
        {
            int suffixNum = 0;
            while (byteNum >= 1000 && suffixNum < DataSuffixes.Length)
            {
                byteNum /= 1024;
                suffixNum++;
            }
            return byteNum.ToString() + DataSuffixes[suffixNum];
        }


        /// <summary>
        /// Event handler to detect the block button being clicked
        /// </summary>
        private void urlBlockButton_Click(object sender, EventArgs e)
        {
            HttpPortListener.BlockHost(urlBlockTextBox.Text);
            blockedUrlsTextBox.Text += "\r\n" + urlBlockTextBox.Text;
            urlBlockTextBox.Text = "";
        }


        #region Automatically Generated and unused code
        private void blockedUrlsTextBox_TextChanged(object sender, EventArgs e)
        {

        }


        //Auto-generated code
        private void cacheSizeTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        //Auto-generated
        private void cacheInfoTextBox_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
