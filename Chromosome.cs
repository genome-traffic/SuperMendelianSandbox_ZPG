﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DrivesZPG
{
    class Chromosome
    {
        public List<GeneLocus> GeneLocusList;
        public string ChromosomeName;
        public string HomologousPairName;

        public Chromosome(string CName, string PName)
        {
            this.ChromosomeName = CName;
            this.HomologousPairName = PName;
            this.GeneLocusList = new List<GeneLocus>();
        }

        //Clone a Chromsome
        public Chromosome(Chromosome Old)
        {
            this.ChromosomeName = Old.ChromosomeName;
            this.HomologousPairName = Old.HomologousPairName;
            this.GeneLocusList = new List<GeneLocus>();

            foreach (GeneLocus OldGL in Old.GeneLocusList)
            {
                GeneLocus NewGL = new GeneLocus(OldGL);
                GeneLocusList.Add(NewGL);
            }


        }

        //New Chromosome in Meiosis
        public Chromosome(Chromosome HomChrom1, Chromosome HomChrom2, bool homing, int Cas9level)
        {
            if (HomChrom1.HomologousPairName != HomChrom2.HomologousPairName)
            { throw new System.ArgumentException("Not homologous Chromosomes", "warning");}


            if (HomChrom1.HomologousPairName == "Sex")
            {
                if (Simulation.random.Next(0, 2) != 0)
                {
                    this.ChromosomeName = HomChrom1.ChromosomeName;
                    this.HomologousPairName = HomChrom1.HomologousPairName;
                    this.GeneLocusList = new List<GeneLocus>();

                    foreach (GeneLocus OldGL in HomChrom1.GeneLocusList)
                    {
                        GeneLocus NewGL = new GeneLocus(OldGL);
                        GeneLocusList.Add(NewGL);
                    }
                }
                else
                {
                    this.ChromosomeName = HomChrom2.ChromosomeName;
                    this.HomologousPairName = HomChrom2.HomologousPairName;
                    this.GeneLocusList = new List<GeneLocus>();

                    foreach (GeneLocus OldGL in HomChrom2.GeneLocusList)
                    {
                        GeneLocus NewGL = new GeneLocus(OldGL);
                        GeneLocusList.Add(NewGL);
                    }

                }
            }
            else
            {
                this.ChromosomeName = HomChrom1.ChromosomeName;
                this.HomologousPairName = HomChrom1.HomologousPairName;

                Chromosome HC1 = new Chromosome(HomChrom1);
                Chromosome HC2 = new Chromosome(HomChrom2);

                #region homing at all loci
                if (Cas9level > 0 && homing)
                {
                    for (var i = 0; i < HC1.GeneLocusList.Count; i++)
                    {
                        if (HC1.GeneLocusList[i].GeneName == HC2.GeneLocusList[i].GeneName)
                        {
                            if (HC1.GeneLocusList[i].AlleleName == "Drive")
                            {
                                if (HC2.GeneLocusList[i].AlleleName == "WT")
                                {
                                    if (Cas9level >= Simulation.random.Next(0, 100))
                                    {
                                        int Hom_Repair = 0;
                                        int Cons = 0;

                                        HC1.GeneLocusList[i].Traits.TryGetValue("Hom_Repair", out Hom_Repair);
                                        HC2.GeneLocusList[i].Traits.TryGetValue("Conservation", out Cons);

                                        if (Hom_Repair >= Simulation.random.Next(0, 100))
                                        {
                                            HC2.GeneLocusList[i].AlleleName = "Drive";
                                            HC2.GeneLocusList[i].InheritTraits(HC1.GeneLocusList[i]);
                                        }
                                        else
                                        {
                                            if (Cons >= Simulation.random.Next(0, 100))
                                            HC2.GeneLocusList[i].AlleleName = "R2";
                                            else
                                            HC2.GeneLocusList[i].AlleleName = "R1";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (var i = 0; i < HC2.GeneLocusList.Count; i++)
                    {
                        if (HC1.GeneLocusList[i].GeneName == HC2.GeneLocusList[i].GeneName)
                        {
                            if (HC2.GeneLocusList[i].AlleleName == "Drive")
                            {
                                if (HC1.GeneLocusList[i].AlleleName == "WT")
                                {
                                    if (Cas9level >= Simulation.random.Next(0, 100))
                                    {
                                        int Hom_Repair = 0;
                                        int Cons = 0;

                                        HC2.GeneLocusList[i].Traits.TryGetValue("Hom_Repair", out Hom_Repair);
                                        HC1.GeneLocusList[i].Traits.TryGetValue("Conservation", out Cons);

                                        if (Hom_Repair >= Simulation.random.Next(0, 100))
                                        {
                                            HC1.GeneLocusList[i].AlleleName = "Drive";
                                            HC1.GeneLocusList[i].InheritTraits(HC2.GeneLocusList[i]);
                                        }
                                        else
                                        {
                                            if (Cons >= Simulation.random.Next(0, 100))
                                            HC1.GeneLocusList[i].AlleleName = "R2";
                                            else
                                            HC1.GeneLocusList[i].AlleleName = "R1";
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                #endregion

                this.GeneLocusList = new List<GeneLocus>();

                #region recombining the two homologous chroms to create new chrom

                for (var i = 0; i < HC1.GeneLocusList.Count; i++)
                {
                    if (Simulation.random.Next(0, 2) != 0)
                    {
                        this.GeneLocusList.Add(new GeneLocus(HC1.GeneLocusList[i]));
                    }
                    else
                    {
                        this.GeneLocusList.Add(new GeneLocus(HC2.GeneLocusList[i]));
                    }
                }
            }
            #endregion

        }

    }

}
