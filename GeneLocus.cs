﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DrivesZPG
{
    class GeneLocus
    {
        // Properties
        public string GeneName;
        public int GenePosition;
        public string AlleleName;
      
        //public List<(string, int)> Traits;
        public Dictionary<string, int> Traits;

        //Constructor
        public GeneLocus(string iGeneName, int iGenePosition, string iAlleleName)
        {
            this.GeneName = iGeneName;
            this.GenePosition = iGenePosition;
            this.AlleleName = iAlleleName;
            //this.GeneProrperty1Name = iGeneProrperty1Name;
            //this.GeneProrperty1Value = iGeneProrperty1Value;
            this.Traits = new Dictionary<string, int>();
            //this.Traits.Add(("Empty",1));

        }
        //Copy Constructor
        public GeneLocus(GeneLocus Old)
        {
            this.AlleleName = Old.AlleleName;
            this.GeneName = Old.GeneName;
            this.GenePosition = Old.GenePosition;

            this.Traits = new Dictionary<string, int>();
            foreach (var OldTrait in Old.Traits)
            {
                this.Traits.Add(OldTrait.Key, OldTrait.Value);        
            }
        }

        public void InheritTraits(GeneLocus Parent)
        {
            this.Traits.Clear();
            foreach (var ParentTrait in Parent.Traits)
            {
                
                this.Traits.Add(ParentTrait.Key, ParentTrait.Value);
            }
        }
    }
}
