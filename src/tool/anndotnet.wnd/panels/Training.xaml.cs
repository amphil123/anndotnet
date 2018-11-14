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
using anndotnet.wnd.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZedGraph;

namespace anndotnet.wnd.Panels
{
    /// <summary>
    /// Interaction logic for Training.xaml
    /// </summary>
    public partial class Training : UserControl
    {
        public LineItem lossMinibatchSerie = null;
        public LineItem evalMinibatchSerie = null;
        public LineItem evalTrainingSerie = null;
        public LineItem evalValidationSerie = null;

        public Training()
        {
            InitializeComponent();
            DataContextChanged += Training_DataContextChanged;           
        }

        private void prepareGraphPanel1()
        {
            try
            {
                var zedGraph = this.trainingMinibatchGraph;

                var configCont = this.DataContext as MLConfigController;
                var loss = configCont.LearningParameters.LossFunction.ToString();
                var eval = configCont.LearningParameters.EvaluationFunction.ToString();

                ///chart for training/predicted data
                zedGraph.GraphPane.Title.Text = "Minibatch training";
                zedGraph.GraphPane.XAxis.Title.Text = "iterations";

                zedGraph.SetFont();


                zedGraph.GraphPane.Border = new ZedGraph.Border(System.Drawing.Color.White, 0);
                zedGraph.GraphPane.YAxis.Title.Text = loss;
                zedGraph.GraphPane.YAxis.Title.IsVisible = false;
                //zedGraph.GraphPane.YAxis.Title.FontSpec.FontColor = System.Drawing.Color.Blue;
                setAxesColor(zedGraph.GraphPane.YAxis, System.Drawing.Color.Blue);

                zedGraph.GraphPane.Y2Axis.Title.IsVisible = false;
                zedGraph.GraphPane.Y2Axis.Title.Text = eval;
                setAxesColor(zedGraph.GraphPane.Y2Axis, System.Drawing.Color.Orange);
                //zedGraph.GraphPane.Y2Axis.Title.FontSpec.FontColor = System.Drawing.Color.Orange;

                zedGraph.GraphPane.Y2Axis.IsVisible = true;
                zedGraph.GraphPane.Legend.IsVisible = true;
                zedGraph.GraphPane.Legend.Border = new ZedGraph.Border(System.Drawing.Color.White, 0);
                zedGraph.GraphPane.Title.IsVisible = false;
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        private void prepareGraphPanel2()
        {
            try
            {
                var zedGraph = this.trainingDatasetsGraph;

                var configCont = this.DataContext as MLConfigController;
                var loss = configCont.LearningParameters.LossFunction.ToString();
                var eval = configCont.LearningParameters.EvaluationFunction.ToString();

                ///chart for training/predicted data
                zedGraph.GraphPane.Title.Text = "Datasets Evaluation";
                zedGraph.GraphPane.XAxis.Title.Text = "iterations";

                //left axes
                zedGraph.GraphPane.Border = new ZedGraph.Border(System.Drawing.Color.White, 0);
                zedGraph.GraphPane.YAxis.Title.Text = eval;//$"Training dataset ({eval})";
                zedGraph.GraphPane.YAxis.Title.IsVisible = false;
                setAxesColor(zedGraph.GraphPane.YAxis, System.Drawing.Color.Blue);

                zedGraph.SetFont();

                //right axes
                //zedGraph.GraphPane.Y2Axis.Title.FontSpec.Size = 16.0f;//4.0f * (zedGraph.Size.Width / 100);
                zedGraph.GraphPane.Y2Axis.Title.Text = eval;// $"Validation dataset ({eval})";
                zedGraph.GraphPane.Y2Axis.Title.IsVisible = false;
                setAxesColor(zedGraph.GraphPane.Y2Axis, System.Drawing.Color.Orange);


                //legend
                zedGraph.GraphPane.Legend.IsVisible = true;
                zedGraph.GraphPane.Legend.Border = new ZedGraph.Border(System.Drawing.Color.White, 0);
                zedGraph.GraphPane.Title.IsVisible = false;

                chart2Title.Text = $"{eval} ";
            }
            catch (Exception)
            {

               // throw;
            }
            

          

        }

        private void setAxesColor(Axis axes, Color color)
        {
            axes.Color = color;
            axes.MajorGrid.Color = color;
            axes.MinorGrid.Color = color;
            axes.MajorTic.Color = color;
            axes.MinorTic.Color = color;
            axes.Scale.FontSpec.FontColor = color;
            axes.Title.FontSpec.FontColor = color;

        }

        private void Training_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //clear prev. state
                trainingMinibatchGraph.GraphPane.CurveList.Clear();
                trainingDatasetsGraph.GraphPane.CurveList.Clear();

                //force to update the control
                this.trainingMinibatchGraph.RestoreScale(trainingMinibatchGraph.GraphPane);
                this.trainingDatasetsGraph.RestoreScale(trainingDatasetsGraph.GraphPane);

                if (this.DataContext == null || !(this.DataContext is MLConfigController))
                    return;
                var mlConfig = this.DataContext as MLConfigController;

                //
                mlConfig.UpdateTrainingtGraphs = UpdateGraphs;

                //prepage graph
                prepareGraphPanel1();
                prepareGraphPanel2();
                //
                preparesSeriesGraph1();
                bool isValidDefined = mlConfig.IsValidationSetDefined();
                preparesSeriesGraph2(isValidDefined);

                //
                for (int i = 0; i < mlConfig.TrainingProgress.MBLossValue.Count; i++)
                {
                    lossMinibatchSerie.AddPoint(mlConfig.TrainingProgress.MBLossValue[i]);
                    evalMinibatchSerie.AddPoint(mlConfig.TrainingProgress.MBEvaluationValue[i]);
                }
                for (int i = 0; i < mlConfig.TrainingProgress.TrainEvalValue.Count; i++)
                {
                    evalTrainingSerie.AddPoint(mlConfig.TrainingProgress.TrainEvalValue[i]);
                    evalValidationSerie.AddPoint(mlConfig.TrainingProgress.ValidationEvalValue[i]);
                }

                //Refresh the charts
                trainingMinibatchGraph.RestoreScale(trainingMinibatchGraph.GraphPane);
                trainingDatasetsGraph.RestoreScale(trainingDatasetsGraph.GraphPane);
            }
            catch (Exception ex)
            {

                var ac = App.Current.MainWindow.DataContext as AppController;
                if (ac != null)
                    ac.ReportException(ex);
            }
            

        }
        public void UpdateGraphs(int it, double lossMB, double evaMB, double evalTrain, double evalValid)
        {
            if(it==0)
            {
                //reste grapphs
                //clear prev. state
                trainingMinibatchGraph.GraphPane.CurveList.Clear();
                trainingDatasetsGraph.GraphPane.CurveList.Clear();
                //
                preparesSeriesGraph1();
                var mlConfig = this.DataContext as MLConfigController;
                bool isValidDefined = mlConfig.IsValidationSetDefined();
                preparesSeriesGraph2(isValidDefined);
            }
            if(it==1)
            {
                var configCont = this.DataContext as MLConfigController;
                var loss = configCont.LearningParameters.LossFunction.ToString();
                var eval = configCont.LearningParameters.EvaluationFunction.ToString();

                //
                trainingDatasetsGraph.GraphPane.YAxis.Title.Text = $"Training dataset ({eval})";
                trainingDatasetsGraph.GraphPane.Y2Axis.Title.Text = $"Validation dataset ({eval})";
            }
            lossMinibatchSerie.AddPoint(new PointPair(it, lossMB));
            evalMinibatchSerie.AddPoint(new PointPair(it, evaMB));
            evalTrainingSerie.AddPoint(new PointPair(it, evalTrain));
            evalValidationSerie.AddPoint(new PointPair(it, evalValid));

            //Refresh the charts
            trainingMinibatchGraph.RestoreScale(trainingMinibatchGraph.GraphPane);
            trainingDatasetsGraph.RestoreScale(trainingDatasetsGraph.GraphPane);


        }
        void preparesSeriesGraph1()
        {
           var zedGraph = this.trainingMinibatchGraph;

            var configCont = this.DataContext as MLConfigController;
            var loss = configCont.LearningParameters.LossFunction.ToString();
            var eval = configCont.LearningParameters.EvaluationFunction.ToString();

            lossMinibatchSerie = new LineItem($"Loss ({loss})", null, null, System.Drawing.Color.Blue, ZedGraph.SymbolType.None, 1);
            lossMinibatchSerie.Symbol.Fill = new Fill(System.Drawing.Color.Blue);
            lossMinibatchSerie.Symbol.Size = 1;
            // Make it a smooth line
            lossMinibatchSerie.Line.IsSmooth = true;
            lossMinibatchSerie.Line.SmoothTension = 0.9F;
            lossMinibatchSerie.Line.Width = 2;
            //
            evalMinibatchSerie = new LineItem($"Evaluation ({eval})", null, null, System.Drawing.Color.Orange, ZedGraph.SymbolType.None, 1);
            evalMinibatchSerie.Symbol.Fill = new Fill(System.Drawing.Color.Orange);
            evalMinibatchSerie.Symbol.Size = 1;
            evalMinibatchSerie.YAxisIndex = 0;
            evalMinibatchSerie.IsY2Axis = true;
            
            // Make it a smooth line
            evalMinibatchSerie.Line.IsSmooth = true;
            evalMinibatchSerie.Line.SmoothTension = 0.9F;
            evalMinibatchSerie.Line.Width = 2;

            zedGraph.GraphPane.YAxis.Scale.MaxAuto = true;
            zedGraph.GraphPane.YAxis.Scale.MinAuto = true;

            zedGraph.GraphPane.Y2Axis.Scale.MaxAuto = true;
            zedGraph.GraphPane.Y2Axis.Scale.MinAuto = true;


            zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = false;
            zedGraph.GraphPane.Y2Axis.MajorGrid.IsVisible = false;
            zedGraph.GraphPane.YAxis.MinorTic.IsOpposite = false;
            zedGraph.GraphPane.Y2Axis.MinorTic.IsOpposite = false;


            zedGraph.GraphPane.YAxis.MinorGrid.IsVisible = false;
            zedGraph.GraphPane.Y2Axis.MinorGrid.IsVisible = false;

            zedGraph.GraphPane.XAxis.Scale.Min = 1.0;
            zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = false;

            
            zedGraph.GraphPane.XAxis.Scale.MajorStep = 1;
            zedGraph.GraphPane.XAxis.Scale.MinorStep = 0;

            //add series after initialization
            zedGraph.GraphPane.CurveList.Add(lossMinibatchSerie);
            zedGraph.GraphPane.CurveList.Add(evalMinibatchSerie);

        }

        void preparesSeriesGraph2(bool isValidationSetDefined)
        {

            var zedGraph = this.trainingDatasetsGraph;

            evalTrainingSerie = new LineItem("Training data set", null, null, System.Drawing.Color.Blue, ZedGraph.SymbolType.None, 1);
            evalTrainingSerie.Symbol.Fill = new Fill(System.Drawing.Color.Blue);
            evalTrainingSerie.Symbol.Size = 1;
            // Make it a smooth line
            evalTrainingSerie.Line.IsSmooth = true;
            evalTrainingSerie.Line.SmoothTension = 0.9F;
            evalTrainingSerie.Line.Width = 1.8f;
            //
            evalValidationSerie = new LineItem("Validation data set", null, null, System.Drawing.Color.Orange, ZedGraph.SymbolType.None, 1);
            evalValidationSerie.Symbol.Fill = new Fill(System.Drawing.Color.Orange);
            evalValidationSerie.Symbol.Size = 1;
            evalValidationSerie.YAxisIndex = 0;
            evalValidationSerie.IsY2Axis = true;
             


            // Make it a smooth line
            evalMinibatchSerie.Line.IsSmooth = true;
            evalMinibatchSerie.Line.SmoothTension = 0.9F;
            evalMinibatchSerie.Line.Width = 2;

            zedGraph.GraphPane.YAxis.Scale.MaxAuto = true;
            zedGraph.GraphPane.YAxis.Scale.MinAuto = true;

            zedGraph.GraphPane.Y2Axis.Scale.MaxAuto = true;
            zedGraph.GraphPane.Y2Axis.Scale.MinAuto = true;


            zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = false;
            zedGraph.GraphPane.Y2Axis.MajorGrid.IsVisible = false;
            zedGraph.GraphPane.YAxis.MinorTic.IsOpposite = false;
            zedGraph.GraphPane.Y2Axis.MinorTic.IsOpposite = false;

            zedGraph.GraphPane.XAxis.Scale.Min = 1.0;
            zedGraph.GraphPane.YAxis.MinorGrid.IsVisible = false;
            zedGraph.GraphPane.Y2Axis.MinorGrid.IsVisible = false;


            zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = false;


            zedGraph.GraphPane.XAxis.Scale.MajorStep = 1;
            zedGraph.GraphPane.XAxis.Scale.MinorStep = 0;


            //add series after initialization
            zedGraph.GraphPane.CurveList.Add(evalTrainingSerie);
            //add series in case Validation dataset is not defined
            if(isValidationSetDefined)
                zedGraph.GraphPane.CurveList.Add(evalValidationSerie);

        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9]+");
        }

     
    }
}
