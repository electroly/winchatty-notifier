namespace Notifier
{
   partial class OptionsForm
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
         System.Windows.Forms.Label label1;
         System.Windows.Forms.Label label2;
         System.Windows.Forms.Label label3;
         this._LinkHandlerCmb = new System.Windows.Forms.ComboBox();
         this._OKBtn = new System.Windows.Forms.Button();
         this._CancelBtn = new System.Windows.Forms.Button();
         this._DurationUpd = new System.Windows.Forms.NumericUpDown();
         label1 = new System.Windows.Forms.Label();
         label2 = new System.Windows.Forms.Label();
         label3 = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this._DurationUpd)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Location = new System.Drawing.Point(12, 15);
         label1.Name = "label1";
         label1.Size = new System.Drawing.Size(79, 15);
         label1.TabIndex = 0;
         label1.Text = "Open links in:";
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Location = new System.Drawing.Point(12, 43);
         label2.Name = "label2";
         label2.Size = new System.Drawing.Size(100, 15);
         label2.TabIndex = 4;
         label2.Text = "Show balloon for:";
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Location = new System.Drawing.Point(175, 43);
         label3.Name = "label3";
         label3.Size = new System.Drawing.Size(50, 15);
         label3.TabIndex = 6;
         label3.Text = "seconds";
         // 
         // _LinkHandlerCmb
         // 
         this._LinkHandlerCmb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this._LinkHandlerCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this._LinkHandlerCmb.FormattingEnabled = true;
         this._LinkHandlerCmb.Items.AddRange(new object[] {
            "Web browser",
            "Lamp"});
         this._LinkHandlerCmb.Location = new System.Drawing.Point(118, 12);
         this._LinkHandlerCmb.Name = "_LinkHandlerCmb";
         this._LinkHandlerCmb.Size = new System.Drawing.Size(123, 23);
         this._LinkHandlerCmb.TabIndex = 1;
         // 
         // _OKBtn
         // 
         this._OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._OKBtn.Location = new System.Drawing.Point(85, 88);
         this._OKBtn.Name = "_OKBtn";
         this._OKBtn.Size = new System.Drawing.Size(75, 23);
         this._OKBtn.TabIndex = 2;
         this._OKBtn.Text = "OK";
         this._OKBtn.UseVisualStyleBackColor = true;
         // 
         // _CancelBtn
         // 
         this._CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._CancelBtn.Location = new System.Drawing.Point(166, 88);
         this._CancelBtn.Name = "_CancelBtn";
         this._CancelBtn.Size = new System.Drawing.Size(75, 23);
         this._CancelBtn.TabIndex = 3;
         this._CancelBtn.Text = "Cancel";
         this._CancelBtn.UseVisualStyleBackColor = true;
         // 
         // _DurationUpd
         // 
         this._DurationUpd.Location = new System.Drawing.Point(118, 41);
         this._DurationUpd.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
         this._DurationUpd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this._DurationUpd.Name = "_DurationUpd";
         this._DurationUpd.Size = new System.Drawing.Size(51, 23);
         this._DurationUpd.TabIndex = 5;
         this._DurationUpd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this._DurationUpd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
         // 
         // OptionsForm
         // 
         this.AcceptButton = this._OKBtn;
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this._CancelBtn;
         this.ClientSize = new System.Drawing.Size(253, 123);
         this.Controls.Add(label3);
         this.Controls.Add(this._DurationUpd);
         this.Controls.Add(label2);
         this.Controls.Add(this._CancelBtn);
         this.Controls.Add(this._OKBtn);
         this.Controls.Add(this._LinkHandlerCmb);
         this.Controls.Add(label1);
         this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "OptionsForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "WinChatty Notifier Options";
         ((System.ComponentModel.ISupportInitialize)(this._DurationUpd)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ComboBox _LinkHandlerCmb;
      private System.Windows.Forms.Button _OKBtn;
      private System.Windows.Forms.Button _CancelBtn;
      private System.Windows.Forms.NumericUpDown _DurationUpd;
   }
}