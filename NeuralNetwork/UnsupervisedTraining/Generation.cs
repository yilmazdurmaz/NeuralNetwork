﻿using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class Generation
    {
        public double[] Evals { get; set; }
        private IList<ITrainingSession> _sessions;
        private readonly GenerationConfigurationSettings _generationConfig;

        public Generation(IList<ITrainingSession> population, GenerationConfigurationSettings generationConfig)
        {
            _sessions = population;
            _generationConfig = generationConfig;
        }

        public void Run()
        {
            if (_generationConfig.UseMultithreading)
            {
                Parallel.ForEach<ITrainingSession>(_sessions, session =>
                {
                    session.Run();
                });
            }
            else
            {
                foreach (var session in _sessions)
                {
                    session.Run();
                }
            }
        }

        public double[] GetEvalsForGeneration()
        {
            if (Evals == null)
            {
                Evals = new double[_sessions.Count];
                for (int i = 0; i < _sessions.Count; i++)
                {
                    Evals[i] = _sessions[i].GetSessionEvaluation();
                }
            }
            //TODO: this shouldn't be in Evals anymore, but just called directly off of the training session
            double[] toReturn = new double[_sessions.Count];
            for (int i = 0; i < _sessions.Count; i++)
            {
                toReturn[i] = Evals[i];
            }
            return toReturn;
        }

        public ITrainingSession GetBestPerformer()
        {
            int indexToKeep = 0;
            for (int performer = 0; performer < Evals.Length; performer++)
            {
                double value = Evals[performer];
                if (value > Evals[indexToKeep])
                {
                    indexToKeep = performer;
                }
            }
            return _sessions[indexToKeep];
        }

        public IList<ITrainingSession> GetBestPerformers(int numPerformers)
        {
            if (numPerformers <= 0)
            {
                throw new ArgumentException("x must be greater than zero");
            }
            if (numPerformers > _sessions.Count)
            {
                throw new ArgumentException("x must be less than number of sesssions");
            }

            int[] indicesToKeep = new int[numPerformers];
            for (int i = 0; i < numPerformers; i++)
            {
                indicesToKeep[i] = i;
            }
            var evals = GetEvalsForGeneration();
            for (int performer = 0; performer < evals.Length; performer++)
            {
                double value = evals[performer];
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    if (value > evals[indicesToKeep[i]])
                    {
                        int newIndex = performer;
                        // need to shift all of the rest down now
                        for (int indexContinued = i; indexContinued < numPerformers; indexContinued++)
                        {
                            int oldIndex = indicesToKeep[indexContinued];
                            indicesToKeep[indexContinued] = newIndex;
                            newIndex = oldIndex;
                        }
                        break;
                    }
                }
            }

            var sessionsToReturn = new List<ITrainingSession>();
            for (int i = 0; i < numPerformers; i++)
            {
                sessionsToReturn.Add(_sessions[indicesToKeep[i]]);
            }
            return sessionsToReturn;
        }
    }
}
