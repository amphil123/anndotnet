﻿//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using ANNdotNET.Core;
using GPdotNet.MathStuff;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace anndotnet.wnd.panels
{
    /// <summary>
    /// Interaction logic for Regression.xaml
    /// </summary>
    public partial class Multiclass : UserControl
    {
        public Multiclass()
        {
            InitializeComponent();
            this.DataContextChanged += BinaryClassification_DataContextChanged;
        }

        private void BinaryClassification_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(this.DataContext is ModelPerformance))
                return;

            var modelPerf = (ModelPerformance)this.DataContext;
            var perfData = ((ModelPerformance)this.DataContext).PerformanceData;
            if (perfData == null)
                return;
            //
            var observation = perfData["obs_train"].Select(x => (int)(double)x).ToArray();
            var prediction = perfData["prd_train"].Select(x => (int)Math.Round((double)x)).ToArray();
            var cm = new ConfusionMatrix(observation, prediction, perfData["Classes"].Count);
            //

            modelPerf.AAcc = (float)Math.Round(ConfusionMatrix.AAC(cm.Matrix),3);
            modelPerf.OAcc = (float)Math.Round(ConfusionMatrix.OAC(cm.Matrix), 3);
            modelPerf.MacPrec = (float)Math.Round(ConfusionMatrix.MacroPrecision(cm.Matrix), 3);
            modelPerf.MicPrec = (float)Math.Round(ConfusionMatrix.MicroPrecision(cm.Matrix), 3);
            modelPerf.MacRcall = (float)Math.Round(ConfusionMatrix.MacroRecall(cm.Matrix), 3);
            modelPerf.MicRcall = (float)Math.Round(ConfusionMatrix.MicroRecall(cm.Matrix), 3);

            modelPerf.HSS = (float)Math.Round(ConfusionMatrix.HSS(cm.Matrix, observation.Length), 3);
            modelPerf.PSS = (float)Math.Round(ConfusionMatrix.PSS(cm.Matrix, observation.Length), 3);
        }

        private void CMatric_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(this.DataContext is ModelPerformance))
                    return;

                var modelPerf = (ModelPerformance)this.DataContext;
                var ret = ((ModelPerformance)this.DataContext).PerformanceData;

                //no validation or 
                if (ret == null)
                {
                    throw new Exception("The dataset is empty!");
                }

                //MModelEvaluation dlg = new MModelEvaluation();
                MClassEvalWnd dlg = new MClassEvalWnd();
                dlg.Title = $"Confusion matrix for {modelPerf.DatSetName}.";
                var cl = ret["Classes"].Select(x => x.ToString()).ToArray();
                dlg.loadClasses(cl);
                dlg.loadData(ret["obs_train"].Select(x => (double)x).ToArray(), ret["prd_train"].Select(x => (double)x).ToArray());

                dlg.ShowDialog();
            }
            catch (Exception ex)
            {


                var ac = App.Current.MainWindow.DataContext as AppController;
                if (ac != null)
                    ac.ReportException(ex);
            }
            
        }
    }
}
