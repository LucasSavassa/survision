using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public readonly record struct Prediction : IPrediction
    {
        public string Name { get; }
        public double Probability { get; }
        public double Left { get; }
        public double Top { get; }
        public double Height { get; }
        public double Width { get; }

        public Prediction(string name, double probability, double left, double top, double height, double width)
        {
            Name = name;
            Probability = probability;
            Left = left;
            Top = top;
            Height = height;
            Width = width;
        }
    }
}
