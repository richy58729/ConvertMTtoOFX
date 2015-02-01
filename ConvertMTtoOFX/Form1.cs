using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertMTtoOFX {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();

      OpenFileDialog InputFileBrowser = new OpenFileDialog();
      InputFileBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Downloads";
      InputFileBrowser.Filter = "SWIFT MT940 files (*.sta)|*.sta";
      InputFileBrowser.Title = "Select MT940 input file";
      InputFileBrowser.ShowDialog();
      if (InputFileBrowser.FileName == "") {
        Environment.Exit(1);
      }

      SaveFileDialog OutputFileBrowser = new SaveFileDialog();
      OutputFileBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Downloads";
      OutputFileBrowser.Filter = "Open Financial Exchange file (*.ofx)|*.ofx";
      OutputFileBrowser.Title = "Save as OFX file";
      OutputFileBrowser.ShowDialog();
      if (OutputFileBrowser.FileName == "") {
        Environment.Exit(2);
      }

      if (InputFileBrowser.CheckFileExists && OutputFileBrowser.CheckPathExists) {
        ProcessStartInfo ProcessInfo = new ProcessStartInfo();
        ProcessInfo.FileName = "java";
        ProcessInfo.UseShellExecute = false;
        ProcessInfo.CreateNoWindow = true;
        ProcessInfo.RedirectStandardError = true;
        ProcessInfo.Arguments = "-jar C:\\Windows\\mtconverter.jar \"" + InputFileBrowser.FileName + "\" \"" + OutputFileBrowser.FileName + "\"";
        Process Java = new Process();
        Java.StartInfo = ProcessInfo;
        Java.Start();
        String Output = Java.StandardError.ReadToEnd();
        Java.WaitForExit();
        if (Output != "") {
          label1.Text = "Conversion went wrong. Sorry!";
          Regex RX = new Regex(@"\(([^\)]+)\)\r\n");
          MatchCollection Matches = RX.Matches(Output);
          if (Matches.Count > 0) {
            GroupCollection Groups = Matches[0].Groups;
            String Error = Groups[1].Value;
            label2.Text = "Error: " + Error;
            label2.Visible = true;
            CenterControlInParent(label2);
          }
        } else {
          this.Controls.Remove(label2);
          tableLayoutPanel1.RowStyles[1].Height = 0;
        }
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      Environment.Exit(0);
    }

    private void Form1_Load(object sender, EventArgs e) {
      this.Activate();
    }

    private void Form1_SizeChanged(object sender, EventArgs e) {
      CenterControlInParent(label1);
    }

    private void CenterControlInParent(Control ctrlToCenter) {
      ctrlToCenter.Left = (ctrlToCenter.Parent.Width - ctrlToCenter.Width) / 2;
      ctrlToCenter.Anchor = AnchorStyles.None;
    }

  }
}
