﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class EvalWorkingSet
    {

        public LinkedList<Double> PastEvals { get; set; }
        public int Size { get; set; }

        public EvalWorkingSet(int size)
        {
            PastEvals = new LinkedList<Double>();
            PastEvals.Add(0.0);
            this.Size = size;
        }

        public void AddEval(double eval)
        {
            PastEvals.AddFirst(eval);
            if (PastEvals.size() > this.Size)
            {
                PastEvals.RemoveLast();
            }
        }

        public bool IsStale()
        {
            if (PastEvals.First <= PastEvals.Last)
            {
                return true;
            }
            return false;
        }

    }
}
