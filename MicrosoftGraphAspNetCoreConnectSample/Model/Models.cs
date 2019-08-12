using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MettingRoom.Model
{
    //enum AttendeeType
    //{
    //    Required
        
    //}


    //enum BodyType
    //{
    //    Html

    //}
    public class Events :Microsoft.Graph.Event
    {

        public List<string> AttenndesMejl { get; set; }

        public bool IsFull { get; set; }


        public List<string> errors { get; set; }
    }


   
}
