using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.IO;


namespace DrivesZPG
{
    class Simulation
    {
        public List<Organism> Adults = new List<Organism>();
        public List<Organism> Eggs = new List<Organism>();
        public static Random random = new Random();
      
        /*-------------------- Simulation Parameters ---------------------------------*/

        public int Generations = 11;
        public int Iterations = 25;

        public int PopulationCap = 600;
        public float Mortality = 0.1f;
        public int GlobalEggsPerFemale = 80;
        public int Sample = 47;

        public bool ApplyIntervention = false;
        public int StartingNumberOfWTFemales = 200;
        public int StartingNumberOfWTMales = 200;
        public int StartIntervention = 2;
        public int EndIntervention = 2;
        public int InterventionReleaseNumber = 100;

        string[] Track = {"ZPG","Aper1","CP","AP2"};
        //string[] Track = { "ZPG" };

        /*------------------------------- The Simulation ---------------------------------------------*/

        public void Simulate()
        { 
            string pathdesktop = (string)Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string pathString = System.IO.Path.Combine(pathdesktop, "Output.csv");
            File.Create(pathString).Dispose();

            Console.WriteLine("Simulation Starts.");

            using (var stream = File.OpenWrite(pathString))
            using (var Fwriter = new StreamWriter(stream))
            {
                // THE ACTUAL SIMULATION
                for (int cIterations = 1; cIterations <= Iterations; cIterations++)
                {
                    Console.WriteLine("Iteration " + cIterations + " out of " + Iterations);
                    Adults.Clear();
                    Eggs.Clear();

                    if (ApplyIntervention)
                        Populate_with_WT();
                    else
                        Populate_with_Setup();

                    Shuffle.ShuffleList(Adults);

                    for (int cGenerations = 1; cGenerations <= Generations; cGenerations++)
                    {
                        if (ApplyIntervention)
                        {
                            if ((cGenerations >= StartIntervention) && (cGenerations <= EndIntervention))
                            {
                                Intervention();
                            }
                        }
                        #region output data to file

                        //------------------------ Genotypes -------

                        List<string> Genotypes = new List<string>();

                         foreach (Organism O in Adults)
                         {
                             foreach (string s in Track)
                             {
                                Genotypes.Add(s + "," + O.GetGenotype(s));
                             }
                         }

                        var queryG = Genotypes.GroupBy(s => s)
                           .Select(g => new { Name = g.Key, Count = g.Count() });

                        foreach (var result in queryG)
                        {
                          Fwriter.WriteLine("{0},{1},{2},{3},all", cIterations, cGenerations, result.Name, result.Count);
                        }

                        Genotypes.Clear();

                        int cSample = Sample;
                        foreach (Organism O in Adults)
                        {
                            if (cSample > 0)
                            {
                                foreach (string s in Track)
                                {
                                    Genotypes.Add(s + "," + O.GetGenotype(s));
                                }
                                cSample--;
                            }
                        }

                        var queryGs = Genotypes.GroupBy(s => s)
                           .Select(g => new { Name = g.Key, Count = g.Count() });

                        foreach (var result in queryGs)
                        {
                            Fwriter.WriteLine("{0},{1},{2},{3},sample", cIterations, cGenerations, result.Name, result.Count);
                        }

                        //------------------------- Sex -----------
                        int numberofallmales = 0;
                        int numberofallfemales = 0;
                        foreach (Organism O in Adults)
                        {
                            if (O.GetSex() == "female")
                                numberofallfemales++;
                            else
                                numberofallmales++;
                        }
                        Fwriter.WriteLine("{0},{1},{2},{3},{4},{5},{6}", cIterations, cGenerations, "Sex", "Males", "NA", numberofallmales, "all");
                        Fwriter.WriteLine("{0},{1},{2},{3},{4},{5},{6}", cIterations, cGenerations, "Sex", "Females", "NA", numberofallfemales, "all");

                        #endregion

                        Shuffle.ShuffleList(Adults);
                        CrossAll();
                        Adults.Clear();
                        Shuffle.ShuffleList(Eggs);

                        #region Return Adults from Eggs for the next Generation
                        int EggsToBeReturned = 0;

                        if (Eggs.Count <= PopulationCap)
                            EggsToBeReturned = Eggs.Count;
                        else
                            EggsToBeReturned = PopulationCap;

                        for (int na = 0; na < EggsToBeReturned; na++)
                        {
                            Adults.Add(new Organism(Eggs[na]));
                        }
                        #endregion

                        Eggs.Clear();

                    }

                }
                // END OF SIMULATION

                Fwriter.Flush();
            }
        }

        //---------------------- Define Organisms, Genotypes and Starting Populations -----------------------------------------------------

        public void Populate_with_WT()
        {
            for (int i = 0; i < StartingNumberOfWTFemales; i++)
            {
                Adults.Add(new Organism(GenerateWTFemale()));
            }
            for (int i = 0; i < StartingNumberOfWTMales; i++)
            {
                Adults.Add(new Organism(GenerateWTMale()));
            }
        }

        public void Populate_with_Setup()
        {
            for (int i = 0; i < 300; i++)
            {
                Adults.Add(new Organism(GenerateWTFemale()));
            }
            for (int i = 0; i < 120; i++)
            {
                Adults.Add(new Organism(GenerateWTMale()));
            }

            for (int i = 0; i < 60; i++)
            {
                Adults.Add(new Organism(GenerateZPG_Aper1_DriveMale()));
                Adults.Add(new Organism(GenerateZPG_AP2_DriveMale()));
                Adults.Add(new Organism(GenerateZPG_CP_DriveMale()));
            }

        }

        public void Intervention()
        {
            for (int i = 0; i < InterventionReleaseNumber; i++)
            {
                Adults.Add(new Organism(GenerateZPG_DriveMale()));
            }
        }

        public Organism GenerateWTFemale()
        {
            Organism WTFemale = new Organism();

            GeneLocus ZPGa = new GeneLocus("ZPG", 1, "WT");
            ZPGa.Traits.Add("Conservation", 95);
            GeneLocus ZPGb = new GeneLocus("ZPG", 1, "WT");
            ZPGb.Traits.Add("Conservation", 95);

            GeneLocus Aper1a = new GeneLocus("Aper1", 2, "WT");
            Aper1a.Traits.Add("Conservation", 95);
            GeneLocus Aper1b = new GeneLocus("Aper1", 2, "WT");
            Aper1b.Traits.Add("Conservation", 95);

            GeneLocus AP2a = new GeneLocus("AP2", 3, "WT");
            AP2a.Traits.Add("Conservation", 95);
            GeneLocus AP2b = new GeneLocus("AP2", 3, "WT");
            AP2b.Traits.Add("Conservation", 95);

            GeneLocus CPa = new GeneLocus("CP", 1, "WT");
            CPa.Traits.Add("Conservation", 95);
            GeneLocus CPb = new GeneLocus("CP", 1, "WT");
            CPb.Traits.Add("Conservation", 95);

            Chromosome ChromXa = new Chromosome("X", "Sex");
            Chromosome ChromXb = new Chromosome("X", "Sex");
            Chromosome Chrom2a = new Chromosome("2", "2");
            Chromosome Chrom2b = new Chromosome("2", "2");
            Chromosome Chrom3a = new Chromosome("3", "3");
            Chromosome Chrom3b = new Chromosome("3", "3");

            Chrom2a.GeneLocusList.Add(ZPGa);
            Chrom2a.GeneLocusList.Add(Aper1a);
            Chrom2a.GeneLocusList.Add(AP2a);
                        
            Chrom2b.GeneLocusList.Add(ZPGb);
            Chrom2b.GeneLocusList.Add(Aper1b);
            Chrom2b.GeneLocusList.Add(AP2b);

            Chrom3a.GeneLocusList.Add(CPa);
            Chrom3b.GeneLocusList.Add(CPb);

            WTFemale.ChromosomeListA.Add(ChromXa);
            WTFemale.ChromosomeListB.Add(ChromXb);
            WTFemale.ChromosomeListA.Add(Chrom2a);
            WTFemale.ChromosomeListB.Add(Chrom2b);
            WTFemale.ChromosomeListA.Add(Chrom3a);
            WTFemale.ChromosomeListB.Add(Chrom3b);

            return WTFemale;
        }

        public Organism GenerateWTMale()
        {
            Organism WTMale = new Organism(GenerateWTFemale());
            Chromosome ChromY = new Chromosome("Y", "Sex");
            GeneLocus MaleFactor = new GeneLocus("MaleDeterminingLocus", 1, "WT");
            ChromY.GeneLocusList.Add(MaleFactor);

            WTMale.ChromosomeListA[0] = ChromY;
      
            return WTMale;
        }

        public Organism GenerateZPG_DriveMale()
        {
            Organism ZPG_Male = new Organism(GenerateWTMale());

            GeneLocus ZPG_d = new GeneLocus("ZPG", 1, "Drive");
            ZPG_d.Traits.Add("Cas9level", 99);
            ZPG_d.Traits.Add("Hom_Repair", 95);

            Organism.ModifyAllele(ref ZPG_Male.ChromosomeListA, ZPG_d, "WT");
            //Organism.ModifyAllele(ref ZPG_Male.ChromosomeListA, ZPG_d, "R1");

            return ZPG_Male;           
        }

        public Organism GenerateZPG_Aper1_DriveMale()
        {
            Organism ZPG_Aper1_Male = new Organism(GenerateZPG_DriveMale());

            GeneLocus Aper1_d = new GeneLocus("Aper1", 2, "Drive");
            Aper1_d.Traits.Add("Cas9level", 0);
            Aper1_d.Traits.Add("Hom_Repair", 95);

            Organism.ModifyAllele(ref ZPG_Aper1_Male.ChromosomeListA, Aper1_d, "WT");

            GeneLocus ZPG_R1 = new GeneLocus("ZPG", 1, "R1");
      
            Organism.ModifyAllele(ref ZPG_Aper1_Male.ChromosomeListB, ZPG_R1, "WT");

            return ZPG_Aper1_Male;
        }

        public Organism GenerateZPG_CP_DriveMale()
        {
            Organism ZPG_CP_Male = new Organism(GenerateZPG_DriveMale());

            GeneLocus CP_d = new GeneLocus("CP", 2, "Drive");
            CP_d.Traits.Add("Cas9level", 0);
            CP_d.Traits.Add("Hom_Repair", 95);

            Organism.ModifyAllele(ref ZPG_CP_Male.ChromosomeListA, CP_d, "WT");

            return ZPG_CP_Male;
        }

        public Organism GenerateZPG_AP2_DriveMale()
        {
            Organism ZPG_AP2_Male = new Organism(GenerateZPG_DriveMale());

            GeneLocus AP2_d = new GeneLocus("AP2", 2, "Drive");
            AP2_d.Traits.Add("Cas9level", 0);
            AP2_d.Traits.Add("Hom_Repair", 95);

            Organism.ModifyAllele(ref ZPG_AP2_Male.ChromosomeListA, AP2_d, "WT");

            return ZPG_AP2_Male;
        }


        //----------------------- Simulation methods ----------------------------------------------------


        public void PerformCross(Organism Dad, Organism Mum, ref List<Organism> EggList)
        {
            int EggsPerFemale = GlobalEggsPerFemale;

            EggsPerFemale = (int)(EggsPerFemale * Dad.GetFertility() * Mum.GetFertility());

                for (int i = 0; i < EggsPerFemale; i++)
                {
                    EggList.Add(new Organism(Dad,Mum));
                }
        }
        public void CrossAll()
        {

            int EffectivePopulation = (int)((1 - Mortality) * PopulationCap);
  
            int numb;
            foreach (Organism F1 in Adults)
            {
                if (F1.GetSex() == "male")
                {
                    continue;
                }
                else
                {
                    for (int a = 0; a < EffectivePopulation; a++)
                    {
                        numb = random.Next(0, Adults.Count);
                        if (Adults[numb].GetSex() == "male")
                        {
                            PerformCross(Adults[numb], F1, ref Eggs);
                            break;
                        }
                    }
                }

            }

        }

    }
}
