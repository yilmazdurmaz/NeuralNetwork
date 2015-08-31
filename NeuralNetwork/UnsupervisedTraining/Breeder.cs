﻿using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class Breeder : IBreeder
    {
        private readonly INeuralNetworkFactory _networkFactory;
        private const double MOTHER_FATHER_BIAS = 0.5;


        public Breeder(INeuralNetworkFactory networkFactory)
        {
            _networkFactory = networkFactory;
        }

        public IList<INeuralNetwork> Breed(IList<ITrainingSession> sessions, int numToBreed)
        {
            WeightedSessionList weightedSessions = new WeightedSessionList(sessions);
            List<INeuralNetwork> children = new List<INeuralNetwork>();
            for (int bred = 0; bred < numToBreed; bred++)
            {
                // choose mother
                ITrainingSession session1 = weightedSessions.ChooseRandomWeightedSession();
                INeuralNetwork mother = session1.NeuralNet;

                // choose father
                ITrainingSession session2 = weightedSessions.ChooseRandomWeightedSession();
                INeuralNetwork father = session2.NeuralNet;

                INeuralNetwork child = mate(mother, father);
                children.Add(child);
            }

            return children;

        }

        internal INeuralNetwork mate(INeuralNetwork mother, INeuralNetwork father)
        {
            NeuralNetworkGene motherGenes = mother.GetGenes();
            NeuralNetworkGene childFatherGenes = father.GetGenes();
            Random random = new Random();

            for (int n = 0; n < childFatherGenes.InputGene.Neurons.Count; n++)
            {
                var neuron = childFatherGenes.InputGene.Neurons[n];
                var motherNeuron = motherGenes.InputGene.Neurons[n];
                childFatherGenes.InputGene.Neurons[n] = BreedNeuron(neuron, motherNeuron, random);
            }

            for (int h = 0; h < childFatherGenes.HiddenGenes.Count; h++)
            {
                for (int j = 0; j < childFatherGenes.HiddenGenes[h].Neurons.Count; j++)
                {
                    var neuron = childFatherGenes.HiddenGenes[h].Neurons[j];
                    var motherNeuron = motherGenes.HiddenGenes[h].Neurons[j];
                    childFatherGenes.HiddenGenes[h].Neurons[j] = BreedNeuron(neuron, motherNeuron, random);
                }
            }

            for (int n = 0; n < childFatherGenes.OutputGene.Neurons.Count; n++)
            {
                var neuron = childFatherGenes.OutputGene.Neurons[n];
                var motherNeuron = motherGenes.OutputGene.Neurons[n];
                childFatherGenes.OutputGene.Neurons[n] = BreedNeuron(neuron, motherNeuron, random);
            }

            INeuralNetwork child = _networkFactory.Create(childFatherGenes);
            return child;
        }

        internal IList<double> BreedAxonWeights(NeuronGene moreTerminals, NeuronGene lessTerminals, Random random)
        {
            var weights = new List<double>();
            for (int j = 0; j < moreTerminals.Axon.Weights.Count; j++)
            {
                if (random.NextDouble() < MOTHER_FATHER_BIAS && j < lessTerminals.Axon.Weights.Count)
                {
                    weights.Add(lessTerminals.Axon.Weights[j]);
                }
                else
                {
                    weights.Add(moreTerminals.Axon.Weights[j]);
                }
            }
            return weights;
        }


        internal NeuronGene BreedNeuron(NeuronGene father, NeuronGene mother, Random random)
        {
            NeuronGene toReturn = new NeuronGene
            {
                Axon = new AxonGene(),
                Soma = new SomaGene()
            };
            if (father.Axon.Weights.Count >= mother.Axon.Weights.Count)
            {
                toReturn.Axon.Weights = BreedAxonWeights(father, mother, random);
            }
            else
            {
                toReturn.Axon.Weights = BreedAxonWeights(mother, father, random);
            }

            if (random.NextDouble() < MOTHER_FATHER_BIAS)
            {
                toReturn.Axon.ActivationFunction = mother.Axon.ActivationFunction;
            }
            else
            {
                toReturn.Axon.ActivationFunction = father.Axon.ActivationFunction;
            }
            if (random.NextDouble() < MOTHER_FATHER_BIAS)
            {
                toReturn.Soma.SummationFunction = mother.Soma.SummationFunction;
            }
            else
            {
                toReturn.Soma.SummationFunction = father.Soma.SummationFunction;
            }
            if (random.NextDouble() < MOTHER_FATHER_BIAS)
            {
                toReturn.Soma.Bias = mother.Soma.Bias;
            }
            else
            {
                toReturn.Soma.Bias = father.Soma.Bias;
            }
            return toReturn;
        }
    }   
}