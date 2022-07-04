
namespace SotnRandoTools
{
    partial class CoopSettingsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.multiplayerPanelTitle = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.optionsBox = new System.Windows.Forms.GroupBox();
            this.shareRelicsCheckbox = new System.Windows.Forms.CheckBox();
            this.shareLocationsCheckbox = new System.Windows.Forms.CheckBox();
            this.sendAssistsCheckbox = new System.Windows.Forms.CheckBox();
            this.shareWarpsCheckbox = new System.Windows.Forms.CheckBox();
            this.sendItemsCheckbox = new System.Windows.Forms.CheckBox();
            this.connectionGroup = new System.Windows.Forms.GroupBox();
            this.saveServerCheckbox = new System.Windows.Forms.CheckBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.multiplayerComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sendMayhemCommandsCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.receiveMayhemCommandsCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.optionsBox.SuspendLayout();
            this.connectionGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplayerPanelTitle
            // 
            this.multiplayerPanelTitle.AutoSize = true;
            this.multiplayerPanelTitle.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.multiplayerPanelTitle.Location = new System.Drawing.Point(1, 0);
            this.multiplayerPanelTitle.Name = "multiplayerPanelTitle";
            this.multiplayerPanelTitle.Size = new System.Drawing.Size(263, 29);
            this.multiplayerPanelTitle.TabIndex = 0;
            this.multiplayerPanelTitle.Text = "Co-op / Versus Settings";
            // 
            // divider
            // 
            this.divider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider.Location = new System.Drawing.Point(6, 38);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(382, 2);
            this.divider.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.saveButton.FlatAppearance.BorderSize = 2;
            this.saveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveButton.Location = new System.Drawing.Point(304, 340);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(84, 25);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // optionsBox
            // 
            this.optionsBox.Controls.Add(this.shareRelicsCheckbox);
            this.optionsBox.Controls.Add(this.shareLocationsCheckbox);
            this.optionsBox.Controls.Add(this.sendAssistsCheckbox);
            this.optionsBox.Controls.Add(this.shareWarpsCheckbox);
            this.optionsBox.Controls.Add(this.sendItemsCheckbox);
            this.optionsBox.ForeColor = System.Drawing.Color.White;
            this.optionsBox.Location = new System.Drawing.Point(19, 210);
            this.optionsBox.Name = "optionsBox";
            this.optionsBox.Size = new System.Drawing.Size(112, 141);
            this.optionsBox.TabIndex = 4;
            this.optionsBox.TabStop = false;
            this.optionsBox.Text = "Co-Op Options";
            // 
            // shareRelicsCheckbox
            // 
            this.shareRelicsCheckbox.AutoSize = true;
            this.shareRelicsCheckbox.Location = new System.Drawing.Point(5, 43);
            this.shareRelicsCheckbox.Name = "shareRelicsCheckbox";
            this.shareRelicsCheckbox.Size = new System.Drawing.Size(81, 17);
            this.shareRelicsCheckbox.TabIndex = 5;
            this.shareRelicsCheckbox.Text = "Share relics";
            this.shareRelicsCheckbox.UseVisualStyleBackColor = true;
            this.shareRelicsCheckbox.CheckedChanged += new System.EventHandler(this.shareRelicsCheckbox_CheckedChanged);
            // 
            // shareLocationsCheckbox
            // 
            this.shareLocationsCheckbox.AutoSize = true;
            this.shareLocationsCheckbox.Location = new System.Drawing.Point(5, 20);
            this.shareLocationsCheckbox.Name = "shareLocationsCheckbox";
            this.shareLocationsCheckbox.Size = new System.Drawing.Size(99, 17);
            this.shareLocationsCheckbox.TabIndex = 4;
            this.shareLocationsCheckbox.Text = "Share locations";
            this.shareLocationsCheckbox.UseVisualStyleBackColor = true;
            this.shareLocationsCheckbox.CheckedChanged += new System.EventHandler(this.shareLocationsCheckbox_CheckedChanged);
            // 
            // sendAssistsCheckbox
            // 
            this.sendAssistsCheckbox.AutoSize = true;
            this.sendAssistsCheckbox.Location = new System.Drawing.Point(5, 89);
            this.sendAssistsCheckbox.Name = "sendAssistsCheckbox";
            this.sendAssistsCheckbox.Size = new System.Drawing.Size(85, 17);
            this.sendAssistsCheckbox.TabIndex = 3;
            this.sendAssistsCheckbox.Text = "Send assists";
            this.sendAssistsCheckbox.UseVisualStyleBackColor = true;
            this.sendAssistsCheckbox.CheckedChanged += new System.EventHandler(this.sendAssistsCheckbox_CheckedChanged);
            // 
            // shareWarpsCheckbox
            // 
            this.shareWarpsCheckbox.AutoSize = true;
            this.shareWarpsCheckbox.Location = new System.Drawing.Point(5, 66);
            this.shareWarpsCheckbox.Name = "shareWarpsCheckbox";
            this.shareWarpsCheckbox.Size = new System.Drawing.Size(86, 17);
            this.shareWarpsCheckbox.TabIndex = 1;
            this.shareWarpsCheckbox.Text = "Share warps";
            this.shareWarpsCheckbox.UseVisualStyleBackColor = true;
            this.shareWarpsCheckbox.CheckedChanged += new System.EventHandler(this.shareWarpsCheckbox_CheckedChanged);
            // 
            // sendItemsCheckbox
            // 
            this.sendItemsCheckbox.AutoSize = true;
            this.sendItemsCheckbox.Location = new System.Drawing.Point(5, 112);
            this.sendItemsCheckbox.Name = "sendItemsCheckbox";
            this.sendItemsCheckbox.Size = new System.Drawing.Size(78, 17);
            this.sendItemsCheckbox.TabIndex = 0;
            this.sendItemsCheckbox.Text = "Send items";
            this.sendItemsCheckbox.UseVisualStyleBackColor = true;
            this.sendItemsCheckbox.CheckedChanged += new System.EventHandler(this.sendItemsCheckbox_CheckedChanged);
            // 
            // connectionGroup
            // 
            this.connectionGroup.Controls.Add(this.saveServerCheckbox);
            this.connectionGroup.Controls.Add(this.serverLabel);
            this.connectionGroup.Controls.Add(this.serverTextBox);
            this.connectionGroup.Controls.Add(this.portLabel);
            this.connectionGroup.Controls.Add(this.portTextBox);
            this.connectionGroup.ForeColor = System.Drawing.Color.White;
            this.connectionGroup.Location = new System.Drawing.Point(206, 56);
            this.connectionGroup.Name = "connectionGroup";
            this.connectionGroup.Size = new System.Drawing.Size(182, 110);
            this.connectionGroup.TabIndex = 5;
            this.connectionGroup.TabStop = false;
            this.connectionGroup.Text = "Connection";
            // 
            // saveServerCheckbox
            // 
            this.saveServerCheckbox.AutoSize = true;
            this.saveServerCheckbox.Location = new System.Drawing.Point(6, 47);
            this.saveServerCheckbox.Name = "saveServerCheckbox";
            this.saveServerCheckbox.Size = new System.Drawing.Size(104, 17);
            this.saveServerCheckbox.TabIndex = 4;
            this.saveServerCheckbox.Text = "Save last server";
            this.saveServerCheckbox.UseVisualStyleBackColor = true;
            this.saveServerCheckbox.CheckedChanged += new System.EventHandler(this.saveServerCheckbox_CheckedChanged);
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(6, 77);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(80, 13);
            this.serverLabel.TabIndex = 3;
            this.serverLabel.Text = "Default server:";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(91, 74);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(85, 21);
            this.serverTextBox.TabIndex = 2;
            this.serverTextBox.UseSystemPasswordChar = true;
            this.serverTextBox.TextChanged += new System.EventHandler(this.serverTextBox_TextChanged);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(6, 20);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(69, 13);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Default port:";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(92, 17);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(85, 21);
            this.portTextBox.TabIndex = 0;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // multiplayerComboBox
            // 
            this.multiplayerComboBox.AutoCompleteCustomSource.AddRange(new string[] {
            "\"Easy\"",
            "\"Mayhem\"",
            "\"Hard\"",
            "\"Harder\"",
            "\"Base SotN (Off)\""});
            this.multiplayerComboBox.FormattingEnabled = true;
            this.multiplayerComboBox.Items.AddRange(new object[] {
            "Custom",
            "Co-op",
            "Vs."});
            this.multiplayerComboBox.Location = new System.Drawing.Point(6, 43);
            this.multiplayerComboBox.Name = "multiplayerComboBox";
            this.multiplayerComboBox.Size = new System.Drawing.Size(121, 21);
            this.multiplayerComboBox.TabIndex = 74;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sendMayhemCommandsCheckbox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.receiveMayhemCommandsCheckbox);
            this.groupBox1.Controls.Add(this.multiplayerComboBox);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(19, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 141);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shared Settings";
            // 
            // sendMayhemCommandsCheckbox
            // 
            this.sendMayhemCommandsCheckbox.AutoSize = true;
            this.sendMayhemCommandsCheckbox.Location = new System.Drawing.Point(6, 89);
            this.sendMayhemCommandsCheckbox.Name = "sendMayhemCommandsCheckbox";
            this.sendMayhemCommandsCheckbox.Size = new System.Drawing.Size(148, 17);
            this.sendMayhemCommandsCheckbox.TabIndex = 75;
            this.sendMayhemCommandsCheckbox.Text = "Send Mayhem Commands";
            this.sendMayhemCommandsCheckbox.UseVisualStyleBackColor = true;
            this.sendMayhemCommandsCheckbox.CheckedChanged += new System.EventHandler(this.sendMayhemCommandsCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Game Mode:";
            // 
            // receiveMayhemCommandsCheckbox
            // 
            this.receiveMayhemCommandsCheckbox.AutoSize = true;
            this.receiveMayhemCommandsCheckbox.Location = new System.Drawing.Point(6, 112);
            this.receiveMayhemCommandsCheckbox.Name = "receiveMayhemCommandsCheckbox";
            this.receiveMayhemCommandsCheckbox.Size = new System.Drawing.Size(162, 17);
            this.receiveMayhemCommandsCheckbox.TabIndex = 4;
            this.receiveMayhemCommandsCheckbox.Text = "Receive Mayhem Commands";
            this.receiveMayhemCommandsCheckbox.UseVisualStyleBackColor = true;
            this.receiveMayhemCommandsCheckbox.CheckedChanged += new System.EventHandler(this.receiveMayhemCommandsCheckbox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox3);
            this.groupBox2.Controls.Add(this.checkBox4);
            this.groupBox2.Controls.Add(this.checkBox5);
            this.groupBox2.Controls.Add(this.checkBox6);
            this.groupBox2.Controls.Add(this.checkBox7);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(148, 210);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(116, 141);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Versus Options";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(5, 20);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(59, 17);
            this.checkBox3.TabIndex = 5;
            this.checkBox3.Text = "Send X";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(5, 112);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(59, 17);
            this.checkBox4.TabIndex = 4;
            this.checkBox4.Text = "Send X";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(5, 89);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(59, 17);
            this.checkBox5.TabIndex = 3;
            this.checkBox5.Text = "Send X";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(5, 66);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(59, 17);
            this.checkBox6.TabIndex = 1;
            this.checkBox6.Text = "Send X";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(5, 43);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(59, 17);
            this.checkBox7.TabIndex = 0;
            this.checkBox7.Text = "Send X";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox8);
            this.groupBox3.Controls.Add(this.checkBox11);
            this.groupBox3.Controls.Add(this.checkBox12);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(279, 210);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(103, 93);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Other Options";
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(5, 20);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(63, 17);
            this.checkBox8.TabIndex = 5;
            this.checkBox8.Text = "Share X";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(5, 66);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(63, 17);
            this.checkBox11.TabIndex = 1;
            this.checkBox11.Text = "Share X";
            this.checkBox11.UseVisualStyleBackColor = true;
            // 
            // checkBox12
            // 
            this.checkBox12.AutoSize = true;
            this.checkBox12.Location = new System.Drawing.Point(5, 43);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(63, 17);
            this.checkBox12.TabIndex = 0;
            this.checkBox12.Text = "Share X";
            this.checkBox12.UseVisualStyleBackColor = true;
            // 
            // CoopSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.connectionGroup);
            this.Controls.Add(this.optionsBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.multiplayerPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CoopSettingsPanel";
            this.Size = new System.Drawing.Size(395, 368);
            this.Load += new System.EventHandler(this.MultiplayerSettingsPanel_Load);
            this.optionsBox.ResumeLayout(false);
            this.optionsBox.PerformLayout();
            this.connectionGroup.ResumeLayout(false);
            this.connectionGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label multiplayerPanelTitle;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.GroupBox optionsBox;
		private System.Windows.Forms.CheckBox sendItemsCheckbox;
		private System.Windows.Forms.CheckBox shareWarpsCheckbox;
		private System.Windows.Forms.GroupBox connectionGroup;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.TextBox portTextBox;
		private System.Windows.Forms.Label serverLabel;
		private System.Windows.Forms.TextBox serverTextBox;
		private System.Windows.Forms.CheckBox saveServerCheckbox;
		private System.Windows.Forms.CheckBox shareLocationsCheckbox;
		private System.Windows.Forms.CheckBox sendAssistsCheckbox;
		private System.Windows.Forms.CheckBox shareRelicsCheckbox;
		private System.Windows.Forms.ComboBox multiplayerComboBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox sendMayhemCommandsCheckbox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox receiveMayhemCommandsCheckbox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.CheckBox checkBox7;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBox8;
		private System.Windows.Forms.CheckBox checkBox11;
		private System.Windows.Forms.CheckBox checkBox12;
	}
}
