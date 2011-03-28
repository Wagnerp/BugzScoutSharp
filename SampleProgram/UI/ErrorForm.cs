﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FogCreek;

namespace SampleProgram.UI
{
    public partial class ErrorForm : Form
    {
        private Exception Exception;

        private BugReport Report;

        private const string DefaultReportText = "What were you doing when the error occured?";

        private bool ErrorIsFatal;

        public ErrorForm( Exception e, BugReport report, bool fatal )
        {
            InitializeComponent( );
            this.Exception = e;
            this.Report = report;

            // Style the form differently for fatal errors.
            this.ErrorIsFatal = fatal;
            if ( ErrorIsFatal )
            {
                pbIcon.Image = global::SampleProgram.Properties.Resources.exclamation;
                lblTitle.Text = "We're sorry...";
                lblDescription.Text = "SampleApp has encountered a fatal error and must restart.";
                btnRestart.Text = "Restart";
            }


            Size = MinimumSize;
            UpdateState( );
            BringToFront( );
            bugReporter.RunWorkerAsync( );
        }

        private void UpdateState( )
        {
            lblBottomDivide.Width = Width;

            if ( ErrorIsFatal )
                MainForm.Instance.Hide( );
        }

        private void btnRestart_Click( object sender, EventArgs e )
        {
            if ( ErrorIsFatal )
            {
                ErrorHandler.RestartApplication( );
                Environment.Exit( 0 );
            }
            Close( );
        }

        private void btnExit_Click( object sender, EventArgs e )
        {
            Environment.Exit( -1 );
        }

        private void ErrorForm_Resize( object sender, EventArgs e )
        {
            UpdateState( );
        }

        private void bugReporter_DoWork( object sender, DoWorkEventArgs e )
        {
            e.Result = Report.Submit( );
        }

        private void bugReporter_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            string response = (string) e.Result;
            pbSubmitProgress.Hide( );
            lblSubmitting.Hide( );
            panelReportSent.Show( );

            if ( response.Length > 0 )
            {
                Height += 30;
                lblDeveloperResponse.Text = "Developer response: " + response;
                lblDeveloperResponse.Show( );
            }
        }
    }
}
