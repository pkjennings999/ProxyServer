namespace WebProxy
{
    partial class ManagementConsole
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cacheInfoTextBox = new System.Windows.Forms.TextBox();
            this.averageBytesTextBox = new System.Windows.Forms.TextBox();
            this.bytesFromServerTextBox = new System.Windows.Forms.TextBox();
            this.cacheTimingTextBox = new System.Windows.Forms.TextBox();
            this.requestCountTextBox = new System.Windows.Forms.TextBox();
            this.cacheSizeTextBox = new System.Windows.Forms.TextBox();
            this.bytesFromCacheTextBox = new System.Windows.Forms.TextBox();
            this.urlBlockerTitle = new System.Windows.Forms.TextBox();
            this.urlBlockButton = new System.Windows.Forms.Button();
            this.urlBlockTextBox = new System.Windows.Forms.TextBox();
            this.blockedUrlsTextBox = new System.Windows.Forms.TextBox();
            this.requestsDisplayInfoTextBox = new System.Windows.Forms.TextBox();
            this.requestsMadeTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cacheInfoTextBox
            // 
            this.cacheInfoTextBox.Location = new System.Drawing.Point(12, 12);
            this.cacheInfoTextBox.Name = "cacheInfoTextBox";
            this.cacheInfoTextBox.ReadOnly = true;
            this.cacheInfoTextBox.Size = new System.Drawing.Size(260, 20);
            this.cacheInfoTextBox.TabIndex = 0;
            this.cacheInfoTextBox.Text = "Cache Performance Information";
            this.cacheInfoTextBox.TextChanged += new System.EventHandler(this.cacheInfoTextBox_TextChanged);
            // 
            // averageBytesTextBox
            // 
            this.averageBytesTextBox.Location = new System.Drawing.Point(12, 142);
            this.averageBytesTextBox.Name = "averageBytesTextBox";
            this.averageBytesTextBox.ReadOnly = true;
            this.averageBytesTextBox.Size = new System.Drawing.Size(260, 20);
            this.averageBytesTextBox.TabIndex = 5;
            this.averageBytesTextBox.Text = "Average Bytes per Request from Server: ";
            // 
            // bytesFromServerTextBox
            // 
            this.bytesFromServerTextBox.Location = new System.Drawing.Point(12, 116);
            this.bytesFromServerTextBox.Name = "bytesFromServerTextBox";
            this.bytesFromServerTextBox.ReadOnly = true;
            this.bytesFromServerTextBox.Size = new System.Drawing.Size(260, 20);
            this.bytesFromServerTextBox.TabIndex = 4;
            this.bytesFromServerTextBox.Text = "Total Bytes Sent from Server: ";
            // 
            // cacheTimingTextBox
            // 
            this.cacheTimingTextBox.Location = new System.Drawing.Point(12, 90);
            this.cacheTimingTextBox.Name = "cacheTimingTextBox";
            this.cacheTimingTextBox.ReadOnly = true;
            this.cacheTimingTextBox.Size = new System.Drawing.Size(260, 20);
            this.cacheTimingTextBox.TabIndex = 3;
            this.cacheTimingTextBox.Text = "Average Time per Request: ";
            // 
            // requestCountTextBox
            // 
            this.requestCountTextBox.Location = new System.Drawing.Point(12, 64);
            this.requestCountTextBox.Name = "requestCountTextBox";
            this.requestCountTextBox.ReadOnly = true;
            this.requestCountTextBox.Size = new System.Drawing.Size(260, 20);
            this.requestCountTextBox.TabIndex = 2;
            this.requestCountTextBox.Text = "Total HTTP GET requests: ";
            // 
            // cacheSizeTextBox
            // 
            this.cacheSizeTextBox.Location = new System.Drawing.Point(12, 38);
            this.cacheSizeTextBox.Name = "cacheSizeTextBox";
            this.cacheSizeTextBox.ReadOnly = true;
            this.cacheSizeTextBox.Size = new System.Drawing.Size(260, 20);
            this.cacheSizeTextBox.TabIndex = 1;
            this.cacheSizeTextBox.Text = "Cache Size: ";
            this.cacheSizeTextBox.TextChanged += new System.EventHandler(this.cacheSizeTextBox_TextChanged);
            // 
            // bytesFromCacheTextBox
            // 
            this.bytesFromCacheTextBox.Location = new System.Drawing.Point(12, 168);
            this.bytesFromCacheTextBox.Name = "bytesFromCacheTextBox";
            this.bytesFromCacheTextBox.ReadOnly = true;
            this.bytesFromCacheTextBox.Size = new System.Drawing.Size(260, 20);
            this.bytesFromCacheTextBox.TabIndex = 6;
            this.bytesFromCacheTextBox.Text = "Total Bytes Sent from Cache: ";
            // 
            // urlBlockerTitle
            // 
            this.urlBlockerTitle.Location = new System.Drawing.Point(714, 14);
            this.urlBlockerTitle.Name = "urlBlockerTitle";
            this.urlBlockerTitle.ReadOnly = true;
            this.urlBlockerTitle.Size = new System.Drawing.Size(380, 20);
            this.urlBlockerTitle.TabIndex = 7;
            this.urlBlockerTitle.Text = "URL Blocker | Enter URL to block below";
            // 
            // urlBlockButton
            // 
            this.urlBlockButton.Location = new System.Drawing.Point(1019, 38);
            this.urlBlockButton.Name = "urlBlockButton";
            this.urlBlockButton.Size = new System.Drawing.Size(75, 23);
            this.urlBlockButton.TabIndex = 9;
            this.urlBlockButton.Text = "Block";
            this.urlBlockButton.UseVisualStyleBackColor = true;
            this.urlBlockButton.Click += new System.EventHandler(this.urlBlockButton_Click);
            // 
            // urlBlockTextBox
            // 
            this.urlBlockTextBox.Location = new System.Drawing.Point(714, 40);
            this.urlBlockTextBox.Name = "urlBlockTextBox";
            this.urlBlockTextBox.Size = new System.Drawing.Size(299, 20);
            this.urlBlockTextBox.TabIndex = 8;
            // 
            // blockedUrlsTextBox
            // 
            this.blockedUrlsTextBox.Location = new System.Drawing.Point(714, 66);
            this.blockedUrlsTextBox.Multiline = true;
            this.blockedUrlsTextBox.Name = "blockedUrlsTextBox";
            this.blockedUrlsTextBox.ReadOnly = true;
            this.blockedUrlsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.blockedUrlsTextBox.Size = new System.Drawing.Size(380, 507);
            this.blockedUrlsTextBox.TabIndex = 10;
            this.blockedUrlsTextBox.Text = "Blocked Urls:";
            this.blockedUrlsTextBox.TextChanged += new System.EventHandler(this.blockedUrlsTextBox_TextChanged);
            // 
            // requestsDisplayInfoTextBox
            // 
            this.requestsDisplayInfoTextBox.Location = new System.Drawing.Point(278, 12);
            this.requestsDisplayInfoTextBox.Name = "requestsDisplayInfoTextBox";
            this.requestsDisplayInfoTextBox.ReadOnly = true;
            this.requestsDisplayInfoTextBox.Size = new System.Drawing.Size(430, 20);
            this.requestsDisplayInfoTextBox.TabIndex = 11;
            this.requestsDisplayInfoTextBox.Text = "Requests Made";
            // 
            // requestsMadeTextBox
            // 
            this.requestsMadeTextBox.Location = new System.Drawing.Point(279, 38);
            this.requestsMadeTextBox.Multiline = true;
            this.requestsMadeTextBox.Name = "requestsMadeTextBox";
            this.requestsMadeTextBox.ReadOnly = true;
            this.requestsMadeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.requestsMadeTextBox.Size = new System.Drawing.Size(429, 535);
            this.requestsMadeTextBox.TabIndex = 12;
            // 
            // ManagementConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 585);
            this.Controls.Add(this.requestsMadeTextBox);
            this.Controls.Add(this.requestsDisplayInfoTextBox);
            this.Controls.Add(this.blockedUrlsTextBox);
            this.Controls.Add(this.urlBlockTextBox);
            this.Controls.Add(this.urlBlockButton);
            this.Controls.Add(this.urlBlockerTitle);
            this.Controls.Add(this.bytesFromCacheTextBox);
            this.Controls.Add(this.cacheSizeTextBox);
            this.Controls.Add(this.requestCountTextBox);
            this.Controls.Add(this.cacheTimingTextBox);
            this.Controls.Add(this.bytesFromServerTextBox);
            this.Controls.Add(this.averageBytesTextBox);
            this.Controls.Add(this.cacheInfoTextBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ManagementConsole";
            this.Text = "Managment Console";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox cacheInfoTextBox;

        #endregion

        private System.Windows.Forms.TextBox averageBytesTextBox;
        private System.Windows.Forms.TextBox bytesFromServerTextBox;
        private System.Windows.Forms.TextBox cacheTimingTextBox;
        private System.Windows.Forms.TextBox requestCountTextBox;
        private System.Windows.Forms.TextBox cacheSizeTextBox;
        private System.Windows.Forms.TextBox bytesFromCacheTextBox;
        private System.Windows.Forms.TextBox urlBlockerTitle;
        private System.Windows.Forms.Button urlBlockButton;
        private System.Windows.Forms.TextBox urlBlockTextBox;
        private System.Windows.Forms.TextBox blockedUrlsTextBox;
        private System.Windows.Forms.TextBox requestsDisplayInfoTextBox;
        private System.Windows.Forms.TextBox requestsMadeTextBox;
    }
}

