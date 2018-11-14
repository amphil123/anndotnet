﻿//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using CNTK;
using System.Collections.Generic;

namespace ANNdotNET.Core
{
    /// <summary>
    /// The class implements data returned when the model is evaluated 
    /// </summary>
    public class EvaluationResult
    {
        public List<string> Header { get; set; }
        public List<string> OutputClasses { get; set; }

        public Dictionary<string, List<List<float>>> DataSet { get; set; }

        public List<float> Actual { get; set; }
        public List<float> Predicted { get; set; }
        public List<List<float>> ActualEx { get; set; }
        public List<List<float>> PredictedEx { get; set; }
    }

    public class EvaluationParameters
    {
        public uint MinibatchSize { get; set; }
        public MinibatchSourceEx MBSource { get; set; }
        public List<Variable> Input { get; set; }
        public List<Variable> Ouptut { get; set; }
    }
}
    