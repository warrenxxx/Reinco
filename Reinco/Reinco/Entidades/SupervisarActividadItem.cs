﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinco.Entidades
{
    public class SupervisarActividadItem
    {
        public string item { get; set; }
        public string actividad { get; set; }
        public bool aprobacionSi { get; set; }
        public bool aprobacionNo { get; set; }
        public bool observacionLevantada { get; set; }
    }
}
